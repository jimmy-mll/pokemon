using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Pokemon.Core.Extensions;
using Pokemon.Core.Network.Infrastructure;
using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Network.Transport;
using Serilog;

namespace Pokemon.Core.Network.Dispatching;

/// <inheritdoc />
public sealed class MessageDispatcher : IMessageDispatcher
{
	private static readonly ILogger Logger = Log.ForContext<MessageDispatcher>();

	private readonly ConcurrentDictionary<ushort, (Type Type, Func<object, PokemonSession, PokemonMessage, Task> Delegate)> _sessionHandlers;
	private readonly ConcurrentDictionary<ushort, (Type Type, Func<object, PokemonClient, PokemonMessage, Task> Delegate)> _clientHandlers;
	private readonly IServiceProvider _provider;
	
	public MessageDispatcher() : this(NullServiceProvider.Instance) { }
	
	public MessageDispatcher(IServiceProvider provider)
	{
		_provider = provider;
		_sessionHandlers = new ConcurrentDictionary<ushort, (Type Type, Func<object, PokemonSession, PokemonMessage, Task> Delegate)>();
		_clientHandlers = new ConcurrentDictionary<ushort, (Type Type, Func<object, PokemonClient, PokemonMessage, Task> Delegate)>();
	}

	/// <inheritdoc />
	public void InitializeServer(Assembly assembly)
	{
		foreach (var (type, method) in from type in assembly.GetTypes()
		         from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
		         let attribute = method.GetCustomAttribute<MessageHandlerAttribute>()
		         where attribute is not null
		         select (type, method))
		{
			var parameters = method.GetParameters();
			
			if (parameters.Length is not 2)
				throw new InvalidOperationException("Message handler must have exactly two parameters.");
			
			if (parameters[0].ParameterType != typeof(PokemonSession))
				throw new InvalidOperationException("First parameter of message handler must be of type PokemonClient.");

			var messageId = Convert.ToUInt16(parameters[1].ParameterType.GetField("Identifier")?.GetValue(null));

			var handler = method.CreateDelegate<PokemonSession, PokemonMessage, Task>();
			
			if (!_sessionHandlers.TryAdd(messageId, (type, handler)))
				throw new InvalidOperationException($"Message handler for message {messageId} already exists.");
		}
	}

	/// <inheritdoc />
	public async Task DispatchSessionAsync(PokemonSession session, PokemonMessage message)
	{
		if (!_sessionHandlers.TryGetValue(message.MessageId, out var handler))
		{
			Logger.Warning("No handler found for message {MessageId}", message.MessageId);
			return;
		}

		try
		{
			await handler.Delegate(_provider.GetRequiredService(handler.Type), session, message).ConfigureAwait(false);
			
			Logger.Information("Dispatched message {MessageId} to {Handler}", message.MessageId, handler.Delegate.Method.Name);
		}
		catch (TargetInvocationException e)
		{
			Logger.Error(e, "An exception occurred while dispatching message {MessageId} to {Handler}", message.MessageId, handler.Delegate.Method.Name);
		}
	}
	
	/// <inheritdoc />
	public void InitializeClient(Assembly assembly)
	{
		foreach (var (type, method) in from type in assembly.GetTypes()
		         from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
		         let attribute = method.GetCustomAttribute<MessageHandlerAttribute>()
		         where attribute is not null
		         select (type, method))
		{
			var parameters = method.GetParameters();
			
			if (parameters.Length is not 2)
				throw new InvalidOperationException("Message handler must have exactly two parameters.");
			
			if (parameters[0].ParameterType != typeof(PokemonClient))
				throw new InvalidOperationException("First parameter of message handler must be of type PokemonClient.");
			
			var messageId = Convert.ToUInt16(parameters[1].ParameterType.GetField("Identifier")?.GetValue(null));

			var handler = method.CreateDelegate<PokemonClient, PokemonMessage, Task>();
			
			if (!_clientHandlers.TryAdd(messageId, (type, handler)))
				throw new InvalidOperationException($"Message handler for message {messageId} already exists.");
		}
	}

	/// <inheritdoc />
	public async Task DispatchClientAsync(PokemonClient client, PokemonMessage message)
	{
		if (!_clientHandlers.TryGetValue(message.MessageId, out var handler))
		{
			Logger.Warning("No handler found for message {MessageId}", message.MessageId);
			return;
		}

		try
		{
			await handler.Delegate(_provider.GetRequiredService(handler.Type), client, message).ConfigureAwait(false);
			
			Logger.Information("Dispatched message {MessageId} to {Handler}", message.MessageId, handler.Delegate.Method.Name);
		}
		catch (TargetInvocationException e)
		{
			Logger.Error(e, "An exception occurred while dispatching message {MessageId} to {Handler}", message.MessageId, handler.Delegate.Method.Name);
		}
	}
}