using Nemesis.Core.Network.Metadata;
using Nemesis.Core.Network.Transport;

namespace Nemesis.Core.Network.Dispatcher;

/// <summary>
///     Represents a dispatcher that can be used to invoke message delegates.
/// </summary>
public interface IMessageDispatcher
{
	/// <summary>
	///     Asynchronously invokes the specified message delegate.
	/// </summary>
	/// <param name="session">The session that is associated with the message.</param>
	/// <param name="message">The message that is being dispatched.</param>
	/// <returns>A result that represents the dispatch operation.</returns>
	Task DispatchAsync(BaseSession session, PokemonMessage message);
}