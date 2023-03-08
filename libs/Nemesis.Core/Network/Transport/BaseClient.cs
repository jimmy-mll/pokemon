using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nemesis.Core.Extensions;
using Nemesis.Core.Network.Dispatcher;
using Nemesis.Core.Network.Framing;
using Nemesis.Core.Network.Infrastructure;
using Nemesis.Core.Network.Metadata;
using Nemesis.Core.Network.Options;

namespace Nemesis.Core.Network.Transport;

/// <summary>
///     A network client that represents a connection to a remote endpoint.
/// </summary>
public abstract class BaseClient : IAsyncDisposable
{
	private readonly CancellationTokenSource _cts;
	private readonly ILogger<BaseClient> _logger;
	private readonly IMessageParser _messageParser;
	private readonly IMessagePublisher _messagePublisher;
	private readonly ClientOptions _options;
	private readonly Socket _socket;

	private bool _disposed;
	private IDuplexPipe _pipe = null!;

	/// <summary>
	///     ets the remote endpoint of the underlying client.
	/// </summary>
	public IPEndPoint RemoteEndPoint =>
		(IPEndPoint)_socket.RemoteEndPoint!;

	/// <summary>
	///     Triggered when the session is closed.
	/// </summary>
	public CancellationToken SessionClosed =>
		_cts.Token;

	/// <summary>
	///     Initializes a new instance of the <see cref="BaseClient" /> class.
	/// </summary>
	/// <param name="messageParser">The message parser.</param>
	/// <param name="messagePublisher">The message dispatcher.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="options">The configuration for underlying client.</param>
	protected BaseClient(IMessageParser messageParser, IMessagePublisher messagePublisher, ILogger<BaseClient> logger, IOptions<ClientOptions> options)
	{
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		_messageParser = messageParser;
		_messagePublisher = messagePublisher;
		_logger = logger;
		_options = options.Value;
		_cts = new CancellationTokenSource();
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (_disposed)
			return;

		_disposed = true;

		Disconnect();

		await _pipe.Input.CompleteAsync().ConfigureAwait(false);
		await _pipe.Output.CompleteAsync().ConfigureAwait(false);

		try
		{
			_socket.Shutdown(SocketShutdown.Both);
		}
		catch (SocketException)
		{
			/* ignore */
		}

		_socket.Close();
		_socket.Dispose();
		_cts.Dispose();

		GC.SuppressFinalize(this);
	}

	/// <summary>
	///     Called when the session is connected to the remote endpoint.
	/// </summary>
	public event Action? Connected;

	/// <summary>
	///     Called when the session is disconnected to the remote endpoint.
	/// </summary>
	public event Action? Disconnected;

	/// <summary>
	///     Connects the session to the specified endpoint.
	/// </summary>
	public async Task ConnectAsync()
	{
		var endPoint = new IPEndPoint(IPAddress.Parse(_options.Host), _options.Port);

		try
		{
			await _socket.ConnectAsync(endPoint, _cts.Token).ConfigureAwait(false);

			_logger.LogInformation("Client {Name} connected to {EndPoint}", this, endPoint);

			_pipe = DuplexPipe.Create(_socket);
		}
		catch (SocketException e)
		{
			throw new InvalidOperationException("Failed to connect to the remote endpoint", e);
		}

		ReceiveAsync().FireAndForget();
	}

	private async Task ReceiveAsync()
	{
		_logger.LogInformation("Client ({Name}) connected to {RemoteEndPoint}", this, RemoteEndPoint);

		Connected?.Invoke();

		try
		{
			while (!_cts.IsCancellationRequested)
			{
				var readResult = await _pipe.Input.ReadAsync(_cts.Token).ConfigureAwait(false);
				var buffer = readResult.Buffer;

				if (readResult.IsCanceled)
					break;

				try
				{
					if (_messageParser.TryDecodeMessage(buffer, out var message))
						ThreadPool.QueueUserWorkItem(_ => _messagePublisher.Publish(message, this));
				}
				finally
				{
					_pipe.Input.AdvanceTo(buffer.End, buffer.End);
				}
			}
		}
		catch (Exception e) when (e is OperationCanceledException or ObjectDisposedException)
		{
			/* ignore */
		}
		finally
		{
			_logger.LogInformation("Client ({Name}) disconnected from {RemoteEndPoint}", this, RemoteEndPoint);

			Disconnected?.Invoke();
		}
	}

	/// <summary>
	///     Sends a message to the remote endpoint.
	/// </summary>
	/// <param name="message">The message to send.</param>
	public void Send(PokemonMessage message)
	{
		if (_disposed)
			throw new ObjectDisposedException(nameof(BaseClient));

		if (_cts.IsCancellationRequested)
			return;

		var buffer = _messageParser.TryEncodeMessage(message);

		var flushTask = buffer.IsEmpty
			? _pipe.Output.FlushAsync(_cts.Token)
			: _pipe.Output.WriteAsync(buffer, _cts.Token);

		if (!flushTask.IsCompletedSuccessfully)
			flushTask.FireAndForget();
	}

	/// <summary>
	///     Disconnect the session from the remote endpoint.
	/// </summary>
	/// <param name="delay">The delay before the session is disconnected.</param>
	public void Disconnect(TimeSpan? delay = null)
	{
		if (_cts.IsCancellationRequested)
			return;

		if (delay.HasValue)
			_cts.CancelAfter(delay.Value);
		else
			_cts.Cancel();

		_pipe.Input.CancelPendingRead();
		_pipe.Output.CancelPendingFlush();
	}
}