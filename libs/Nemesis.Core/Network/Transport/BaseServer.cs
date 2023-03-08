using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nemesis.Core.Extensions;
using Nemesis.Core.Network.Collections;
using Nemesis.Core.Network.Dispatcher;
using Nemesis.Core.Network.Framing;
using Nemesis.Core.Network.Options;

namespace Nemesis.Core.Network.Transport;

/// <summary>
///     Represents a tcp server that can be used to listen for incoming connections.
/// </summary>
/// <typeparam name="TSession">The type of the session.</typeparam>
public abstract class BaseServer<TSession>
	where TSession : BaseSession
{
	private readonly CancellationTokenSource _cts;
	private readonly ILogger<BaseServer<TSession>> _logger;
	private readonly IMessageDispatcher _messageDispatcher;
	private readonly IMessageParser _messageParser;
	private readonly ServerOptions _options;
	private readonly ISessionCollection<TSession> _sessions;
	private readonly Socket _socket;

	/// <summary>
	///     Initializes a new instance of the <see cref="BaseServer{TSession}" /> class.
	/// </summary>
	/// <param name="messageParser">The message parser.</param>
	/// <param name="messageDispatcher">The message dispatcher.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="options">The configuration for this underlying server.</param>
	/// <param name="sessions">The session collection.</param>
	protected BaseServer(
		IMessageParser messageParser,
		IMessageDispatcher messageDispatcher,
		ILogger<BaseServer<TSession>> logger,
		IOptions<ServerOptions> options,
		ISessionCollection<TSession> sessions)
	{
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		_cts = new CancellationTokenSource();
		_messageParser = messageParser;
		_messageDispatcher = messageDispatcher;
		_logger = logger;
		_options = options.Value;
		_sessions = sessions;
	}

	/// <summary>
	///     Starts the server asynchronously.
	/// </summary>
	public async Task StartAsync()
	{
		var endPoint = new IPEndPoint(IPAddress.Parse(_options.Host), _options.Port);

		try
		{
			_socket.Bind(endPoint);
		}
		catch (SocketException e)
		{
			_logger.LogError(e, "Failed to bind to {EndPoint}", endPoint);
			return;
		}

		_socket.Listen();

		_logger.LogInformation("Server listening on {EndPoint}", endPoint);

		while (!_cts.IsCancellationRequested)
		{
			var sessionSocket = await _socket.AcceptAsync(_cts.Token).ConfigureAwait(false);

			var session = CreateSession(sessionSocket, _messageParser, _messageDispatcher);

			OnSessionConnectedAsync(session)
				.ContinueWith(_ => session.ReceiveAsync(), _cts.Token)
				.Unwrap()
				.ContinueWith(_ => OnSessionDisconnectedAsync(session), _cts.Token)
				.Unwrap()
				.ContinueWith(_ => session.DisposeAsync().AsTask(), _cts.Token)
				.Unwrap()
				.FireAndForget();
		}
	}

	/// <summary>Initializes a new instance of the <see cref="TSession" /> class.</summary>
	/// <param name="socket">The bound socket.</param>
	/// <param name="messageParser">The message parser.</param>
	/// <param name="messageDispatcher">The message dispatcher.</param>
	protected abstract TSession CreateSession(Socket socket, IMessageParser messageParser, IMessageDispatcher messageDispatcher);

	/// <summary>
	///     Called when a session is connected.
	/// </summary>
	/// <param name="session">The connected session.</param>
	protected virtual Task OnSessionConnectedAsync(TSession session)
	{
		_logger.LogInformation("Session ({Name}) connected to {EndPoint}", session, session.RemoteEndPoint);
		_sessions.Add(session);
		return Task.CompletedTask;
	}

	/// <summary>
	///     Called when a session is disconnected.
	/// </summary>
	/// <param name="session">The session will be disconnected.</param>
	protected virtual Task OnSessionDisconnectedAsync(TSession session)
	{
		_sessions.Remove(session);
		_logger.LogInformation("Session ({Name}) disconnected from {EndPoint}", session, session.RemoteEndPoint);
		return Task.CompletedTask;
	}
}