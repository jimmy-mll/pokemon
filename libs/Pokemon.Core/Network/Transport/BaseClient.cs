using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pokemon.Core.Extensions;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Infrastructure;
using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Network.Options;

namespace Pokemon.Core.Network.Transport;

public abstract class BaseClient : IAsyncDisposable
{
	private readonly CancellationTokenSource _cts;
	private readonly ILogger<BaseClient> _logger;
	private readonly IMessageDispatcher _messageDispatcher;
	private readonly IMessageParser _messageParser;
	private readonly ClientOptions _options;
	private readonly Socket _socket;

	private string? _clientId;
	private bool _disposed;
	private IDuplexPipe _pipe = null!;

	/// <summary>Gets the remote endpoint of the underlying client.</summary>
	public IPEndPoint RemoteEndPoint =>
		(IPEndPoint)_socket.RemoteEndPoint!;

	/// <summary>Gets the unique identifier of the underlying client.</summary>
	public string ClientId =>
		_clientId ??= Uuid.New();

	/// <summary>Triggered when the session is closed.</summary>
	public CancellationToken SessionClosed =>
		_cts.Token;

	/// <summary>Initializes a new instance of the <see cref="BaseClient" /> class.</summary>
	/// <param name="messageParser">The message parser.</param>
	/// <param name="messageDispatcher">The message dispatcher.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="options">The configuration for underlying client.</param>
	protected BaseClient(IMessageParser messageParser, IMessageDispatcher messageDispatcher, ILogger<BaseClient> logger, IOptions<ClientOptions> options)
	{
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		_messageParser = messageParser;
		_messageDispatcher = messageDispatcher;
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

	/// <summary>Connects the session to the specified endpoint.</summary>
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
		await OnConnectedAsync().ConfigureAwait(false);

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
                    if (_messageParser.TryDecodeMessage(buffer, out PokemonMessage? message))
                        await _messageDispatcher.DispatchClientAsync(this, message);
                }
                finally
                {
                    _pipe.Input.AdvanceTo(buffer.End, buffer.End);
                }

                /*				try
                                {
                                    if (_messageParser.TryDecodeMessage(buffer, out var message))
                                        await _messageDispatcher.DispatchClientAsync(this, message).ConfigureAwait(false);

                                    if (readResult.IsCompleted)
                                    {
                                        if (!buffer.IsEmpty)
                                            throw new InvalidOperationException("Incomplete message received");

                                        break;
                                    }
                                }
                                finally
                                {
                                    _pipe.Input.AdvanceTo(buffer.Start, buffer.End);
                                }*/
            }
		}
		catch (Exception e) when (e is OperationCanceledException or ObjectDisposedException)
		{
			/* ignore */
		}
		finally
		{
			await OnDisconnectedAsync().ConfigureAwait(false);
		}
	}

	/// <summary>Asynchronously sends a message to the remote endpoint.</summary>
	/// <param name="message">The message to send.</param>
	public ValueTask SendAsync(PokemonMessage message)
	{
		if (_disposed)
			throw new ObjectDisposedException(nameof(BaseClient));

		if (_cts.IsCancellationRequested)
			return ValueTask.CompletedTask;

		var buffer = _messageParser.TryEncodeMessage(message);

		var flushTask = buffer.IsEmpty
			? _pipe.Output.FlushAsync(_cts.Token)
			: _pipe.Output.WriteAsync(buffer, _cts.Token);

		return !flushTask.IsCompletedSuccessfully
			? FireAndForget(flushTask)
			: ValueTask.CompletedTask;

		static async ValueTask FireAndForget(ValueTask<FlushResult> flushTask) =>
			await flushTask.ConfigureAwait(false);
	}

	/// <summary>Disconnect the session from the remote endpoint.</summary>
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

	/// <summary>Called when the session is connected to the remote endpoint.</summary>
	protected virtual ValueTask OnConnectedAsync()
	{
		_logger.LogInformation("Client ({Name}) connected to {RemoteEndPoint}", this, RemoteEndPoint);
		return ValueTask.CompletedTask;
	}

	/// <summary>Called when the session is disconnected to the remote endpoint.</summary>
	protected virtual ValueTask OnDisconnectedAsync()
	{
		_logger.LogInformation("Client ({Name}) disconnected from {RemoteEndPoint}", this, RemoteEndPoint);
		return ValueTask.CompletedTask;
	}
}