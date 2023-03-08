using System.Collections.Concurrent;
using Nemesis.Core.Network.Metadata;
using Nemesis.Core.Network.Transport;

namespace Nemesis.Core.Network.Dispatcher;

/// <summary>
///     This class is used to map messages to their handlers.
/// </summary>
public sealed class MessageMediator : IMessageSubscriber, IMessagePublisher
{
	private readonly ConcurrentDictionary<Type, List<Action<PokemonMessage, BaseClient>>> _actions;

	/// <summary>
	///     Initializes a new instance of the <see cref="MessageMediator" /> class.
	/// </summary>
	public MessageMediator() =>
		_actions = new ConcurrentDictionary<Type, List<Action<PokemonMessage, BaseClient>>>();

	/// <inheritdoc />
	public void Publish(PokemonMessage message, BaseClient client)
	{
		if (!_actions.TryGetValue(message.GetType(), out var list))
			return;

		Parallel.ForEach(list, action => action(message, client));
	}

	/// <inheritdoc />
	public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : PokemonMessage
	{
		var type = typeof(TMessage);

		if (!_actions.TryGetValue(type, out var list))
		{
			list = new List<Action<PokemonMessage, BaseClient>>();
			_actions.TryAdd(type, list);
		}

		list.Add((message, _) => handler((TMessage)message));
	}

	/// <inheritdoc />
	public void Subscribe<TMessage, TClient>(Action<TMessage, TClient> handler) where TMessage : PokemonMessage where TClient : BaseClient
	{
		var type = typeof(TMessage);

		if (!_actions.TryGetValue(type, out var list))
		{
			list = new List<Action<PokemonMessage, BaseClient>>();
			_actions.TryAdd(type, list);
		}

		list.Add((message, client) => handler((TMessage)message, (TClient)client));
	}
}