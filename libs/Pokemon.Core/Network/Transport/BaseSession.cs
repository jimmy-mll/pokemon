using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Infrastructure;
using Pokemon.Core.Network.Metadata;
using Serilog;

namespace Pokemon.Core.Network.Transport;

/// <summary>A network session that represents a connection to a remote endpoint.</summary>
public abstract class BaseSession : IAsyncDisposable
{
	private readonly CancellationTokenSource _cts;
	private readonly IMessageDispatcher _messageDispatcher;
	private readonly IMessageParser _messageParser;
	private readonly IDuplexPipe _pipe;
	private readonly Socket _socket;
	private readonly IBaseServer _server;

	private bool _disposed;
	private string? _sessionId;

	/// <summary>Gets the unique identifier of the underlying session.</summary>
	public string SessionId =>
		_sessionId ??= Uuid.New();

	/// <summary>Gets the remote endpoint of the underlying session.</summary>
	public IPEndPoint RemoteEndPoint =>
		(IPEndPoint)_socket.RemoteEndPoint!;

	/// <summary>Triggered when the session is closed.</summary>
	public CancellationToken SessionClosed =>
		_cts.Token;

	/// <summary>Determines whether the session is connected.</summary>
	public bool IsConnected =>
		!_disposed && _socket.Connected && !_cts.IsCancellationRequested;

	/// <summary>Initializes a new instance of the <see cref="BaseSession" /> class.</summary>
	/// <param name="socket">The bound socket.</param>
	/// <param name="messageParser">The message parser.</param>
	/// <param name="messageDispatcher">The message dispatcher.</param>
	protected BaseSession(
		Socket socket,
		IBaseServer server,
		IMessageParser messageParser,
		IMessageDispatcher messageDispatcher)
	{
		_socket = socket;
		_server = server;
		_messageParser = messageParser;
		_messageDispatcher = messageDispatcher;
		_cts = new CancellationTokenSource();
		_pipe = DuplexPipe.Create(socket);
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

	internal async Task ReceiveAsync()
	{
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
                        await _messageDispatcher.DispatchServerAsync(_server, this, message);
                }
				finally
				{
                    _pipe.Input.AdvanceTo(buffer.End, buffer.End);
                }
            }
        }
		catch (Exception e) when (e is OperationCanceledException or ObjectDisposedException) { }
		catch (IOException) { Disconnect(); }
		catch (Exception e) { Log.Logger.Error($"An error occured. {e.GetType().Name}: {e.Message} {e.StackTrace}."); }
	}

	/// <summary>Asynchronously sends a message to the remote endpoint.</summary>
	/// <param name="message">The message to send.</param>
	public ValueTask SendAsync(PokemonMessage message)
	{
		if (_disposed)
			throw new ObjectDisposedException(nameof(BaseSession));

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
}