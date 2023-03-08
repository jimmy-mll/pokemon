using Nemesis.Core.Network.Metadata;
using Nemesis.Core.Network.Transport;

namespace Nemesis.Core.Network.Dispatcher;

/// <summary>
///     Represents a way to publish any messages.
/// </summary>
public interface IMessagePublisher
{
	/// <summary>
	///     Publishes a message to all observers.
	/// </summary>
	/// <param name="message">The message to publish.</param>
	/// <param name="client">The client to expose.</param>
	void Publish(PokemonMessage message, BaseClient client);
}