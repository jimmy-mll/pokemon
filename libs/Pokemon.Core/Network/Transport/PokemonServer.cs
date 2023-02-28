using System.Net;
using System.Net.Sockets;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Framing;

namespace Pokemon.Core.Network.Transport;

/// <summary>Represents a tcp server that can be used to listen for incoming connections.</summary>
public sealed class PokemonServer
{
	private readonly Socket _socket;
	private readonly CancellationTokenSource _cts;
	private readonly IMessageDispatcher _messageDispatcher;
	private readonly IMessageParser _messageParser;
	
	public event Func<PokemonSession, Task>? SessionConnected;
	public event Func<PokemonSession, Task>? SessionDisconnected;

	/// <summary>Initializes a new instance of the <see cref="PokemonServer"/> class.</summary>
	/// <param name="messageParser">The message parser.</param>
	/// <param name="messageDispatcher">The message dispatcher.</param>
	public PokemonServer(IMessageParser messageParser, IMessageDispatcher messageDispatcher)
	{
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		_cts = new CancellationTokenSource();
		_messageParser = messageParser;
		_messageDispatcher = messageDispatcher;
	}

	/// <summary>Starts the server asynchronously.</summary>
	public async Task StartAsync(IPEndPoint endPoint)
	{
		try
		{
			_socket.Bind(endPoint);
		}
		catch (SocketException e)
		{
			throw new InvalidOperationException("Unable to bind to the specified endpoint.", e);
		}
		
		_socket.Listen();

		while (!_cts.IsCancellationRequested)
		{
			var sessionSocket = await _socket.AcceptAsync(_cts.Token).ConfigureAwait(false);

			var session = new PokemonSession(sessionSocket, _messageParser, _messageDispatcher);

			if (SessionConnected is not null)
				await SessionConnected(session).ConfigureAwait(false);
			
			_ = session.ReceiveAsync()
				.ContinueWith(_ => SessionDisconnected is not null ? SessionDisconnected(session) : Task.CompletedTask, _cts.Token)
				.Unwrap()
				.ContinueWith(_ => session.DisposeAsync().AsTask(), _cts.Token)
				.Unwrap()
				.ConfigureAwait(false);
		}
	}
}