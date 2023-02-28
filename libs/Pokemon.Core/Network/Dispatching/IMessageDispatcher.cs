using System.Reflection;
using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Network.Transport;

namespace Pokemon.Core.Network.Dispatching;

/// <summary>Represents a dispatcher that can be used to invoke message delegates.</summary>
public interface IMessageDispatcher
{
	/// <summary>Registers all handlers from the given <paramref name="assembly" />.</summary>
	/// <param name="assembly">The assembly to find handlers in.</param>
	void InitializeClient(Assembly assembly);
	
	/// <summary>Registers all handlers from the given <paramref name="assembly" />.</summary>
	/// <param name="assembly">The assembly to find handlers in.</param>
	void InitializeServer(Assembly assembly);

	/// <summary>Asynchronously invokes the specified message delegate.</summary>
	/// <param name="session">The session that is associated with the message.</param>
	/// <param name="message">The message that is being dispatched.</param>
	/// <returns>A result that represents the dispatch operation.</returns>
	Task DispatchServerAsync(BaseSession session, PokemonMessage message);

	/// <summary>Synchronously invokes the specified message delegate.</summary>
	/// <param name="client">The client that is associated with the message.</param>
	/// <param name="message">The message that is being dispatched.</param>
	Task DispatchClientAsync(BaseClient client, PokemonMessage message);
}