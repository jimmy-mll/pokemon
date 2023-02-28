using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Infrastructure;
using Pokemon.Core.Network.Metadata;

namespace Pokemon.Core.Network.Transport;

public sealed class PokemonClient : INetworkPeer, IAsyncDisposable
{
	private IDuplexPipe _pipe = null!;
	private readonly Socket _socket;
	private readonly CancellationTokenSource _cts;
	private readonly IMessageParser _messageParser;
	private readonly IMessageDispatcher _messageDispatcher;

	private bool _disposed;

	public event Func<ValueTask>? Connected; 
	public event Func<ValueTask>? Disconnected; 

	/// <summary>Gets the remote endpoint of the underlying client.</summary>
	public IPEndPoint RemoteEndPoint =>
		(IPEndPoint)_socket.RemoteEndPoint!;

	/// <summary>Triggered when the session is closed.</summary>
	public CancellationToken SessionClosed =>
		_cts.Token;

	/// <summary>Initializes a new instance of the <see cref="PokemonClient"/> class.</summary>
	/// <param name="messageParser">The message parser.</param>
	/// <param name="messageDispatcher">The message dispatcher.</param>
	public PokemonClient(IMessageParser messageParser, IMessageDispatcher messageDispatcher)
	{
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		_messageParser = messageParser;
		_messageDispatcher = messageDispatcher;
		_cts = new CancellationTokenSource();
	}

	/// <summary>Connects the session to the specified endpoint.</summary>
	public async Task ConnectAsync(IPEndPoint endPoint)
	{
		try
		{
			await _socket.ConnectAsync(endPoint, _cts.Token).ConfigureAwait(false);

            _pipe = DuplexPipe.Create(_socket);
        }
		catch (SocketException e)
		{
			throw new InvalidOperationException("Failed to connect to the remote endpoint", e);
		}

		_ = ReceiveAsync().ConfigureAwait(false);
	}

	private async Task ReceiveAsync()
	{
		if (Connected is not null)
			await Connected().ConfigureAwait(false);
		
		try
		{
			while (!_cts.IsCancellationRequested)
			{
				var readResult = await _pipe.Input.ReadAsync(_cts.Token).ConfigureAwait(false);

				if (readResult.IsCanceled)
					break;

				var buffer = readResult.Buffer;

				try
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
				}
			}
		}
		catch (Exception e) when (e is OperationCanceledException or ObjectDisposedException)
		{
			/* ignore */
		}
		finally
		{
			if (Disconnected is not null)
				await Disconnected().ConfigureAwait(false);
		}
	}

	/// <summary>Asynchronously sends a message to the remote endpoint.</summary>
	/// <param name="message">The message to send.</param>
	public ValueTask SendAsync(PokemonMessage message)
	{
		if (_disposed)
			throw new ObjectDisposedException(nameof(PokemonClient));

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
	}
}