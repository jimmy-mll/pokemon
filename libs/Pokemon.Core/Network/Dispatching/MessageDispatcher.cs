using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokemon.Core.Network.Dispatching.Handlers;
using Pokemon.Core.Network.Factory;
using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Network.Transport;

namespace Pokemon.Core.Network.Dispatching;

/// <inheritdoc />
public class MessageDispatcher : IMessageDispatcher
{
	private static readonly Type SessionHandlerType = typeof(SessionHandler<,>);
	private static readonly Type ClientHandlerType = typeof(ClientHandler<,>);
	private readonly ConcurrentDictionary<ushort, Type> _clientHandlers;
	private readonly ILogger<MessageDispatcher> _logger;
	private readonly IMessageFactory _messageFactory;
	private readonly IServiceProvider _provider;

	private readonly ConcurrentDictionary<ushort, Type> _sessionHandlers;

	/// <summary>Initializes a new instance of the <see cref="MessageDispatcher" /> class.</summary>
	/// <param name="provider">The service provider.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="messageFactory">The message factory.</param>
	public MessageDispatcher(IServiceProvider provider, ILogger<MessageDispatcher> logger, IMessageFactory messageFactory)
	{
		_provider = provider;
		_logger = logger;
		_messageFactory = messageFactory;
		_sessionHandlers = new ConcurrentDictionary<ushort, Type>();
		_clientHandlers = new ConcurrentDictionary<ushort, Type>();
	}

	/// <inheritdoc />
	public void InitializeClient(Assembly assembly)
	{
		foreach (var type in from type in assembly.GetTypes()
		         where type.BaseType is not null &&
		               type.BaseType.IsGenericType &&
		               type.BaseType.GetGenericTypeDefinition() == ClientHandlerType
		         select type)
		{
			var parameters = type.BaseType!.GetGenericArguments();

			var messageType = parameters[1];

			var messageId = Convert.ToUInt16(messageType.GetField("Identifier")?.GetValue(null));

			if (_clientHandlers.TryAdd(messageId, type))
				_logger.LogWarning("Duplicate message handler for message {MessageName} in {TypeName}", messageType.Name, type.Name);
		}
	}

	/// <inheritdoc />
	public void InitializeServer(Assembly assembly)
	{
		foreach (var type in from type in assembly.GetTypes()
		         where type.BaseType is not null &&
		               type.BaseType.IsGenericType &&
		               type.BaseType.GetGenericTypeDefinition() == SessionHandlerType
		         select type)
		{
			var parameters = type.BaseType!.GetGenericArguments();

			var messageType = parameters[1];

			var messageId = Convert.ToUInt16(messageType.GetField("Identifier")?.GetValue(null));

			if (_sessionHandlers.TryAdd(messageId, type))
				_logger.LogWarning("Duplicate message handler for message {MessageName} in {TypeName}", messageType.Name, type.Name);
		}
	}

	/// <inheritdoc />
	public async Task DispatchServerAsync(BaseSession session, PokemonMessage message)
	{
		if (_messageFactory.TryGetMessageName(message.MessageId, out var messageName))
		{
			_logger.LogWarning("Unknown message with id {MessageName}", message.MessageId);
			return;
		}

		if (!_sessionHandlers.TryGetValue(message.MessageId, out var type))
		{
			_logger.LogWarning("No handler found for message {MessageName}", message.MessageId);
			return;
		}

		var handler = (SessionHandler)_provider.GetRequiredService(type);

		try
		{
			await handler.Delegate(session, message).ConfigureAwait(false);

			_logger.LogInformation("Dispatched message {MessageName} to {HandlerName}", messageName, type.Name);
		}
		catch (TargetInvocationException e)
		{
			_logger.LogError(e, "An exception occurred while dispatching message {MessageName} to {HandlerName}", messageName, type.Name);
		}
	}

	/// <inheritdoc />
	public async Task DispatchClientAsync(BaseClient client, PokemonMessage message)
	{
		if (_messageFactory.TryGetMessageName(message.MessageId, out var messageName))
		{
			_logger.LogWarning("Unknown message with id {MessageName}", message.MessageId);
			return;
		}

		if (!_sessionHandlers.TryGetValue(message.MessageId, out var type))
		{
			_logger.LogWarning("No handler found for message {MessageName}", message.MessageId);
			return;
		}

		var handler = (ClientHandler)_provider.GetRequiredService(type);

		try
		{
			await handler.Delegate(client, message).ConfigureAwait(false);

			_logger.LogInformation("Dispatched message {MessageName} to {HandlerName}", messageName, type.Name);
		}
		catch (TargetInvocationException e)
		{
			_logger.LogError(e, "An exception occurred while dispatching message {MessageName} to {HandlerName}", messageName, type.Name);
		}
	}
}