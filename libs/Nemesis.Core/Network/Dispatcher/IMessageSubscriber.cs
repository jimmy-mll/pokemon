using Nemesis.Core.Network.Metadata;
using Nemesis.Core.Network.Transport;

namespace Nemesis.Core.Network.Dispatcher;

/// <summary>
///     Represents a way to subscribe to any messages.
/// </summary>
public interface IMessageSubscriber
{
	/// <summary>
	///     Subscribes to a message.
	/// </summary>
	/// <param name="handler">The action to call when the message is received.</param>
	/// <typeparam name="TMessage">The type of the message.</typeparam>
	void Subscribe<TMessage>(Action<TMessage> handler)
		where TMessage : PokemonMessage;

	/// <summary>
	///     Subscribes to a message.
	/// </summary>
	/// <param name="handler">The action to call when the message is received.</param>
	/// <typeparam name="TMessage">The type of the message.</typeparam>
	/// <typeparam name="TClient">The type of the client.</typeparam>
	void Subscribe<TMessage, TClient>(Action<TMessage, TClient> handler)
		where TMessage : PokemonMessage
		where TClient : BaseClient;
}