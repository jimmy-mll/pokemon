using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Network.Transport;

namespace Pokemon.Core.Network.Dispatching.Handlers;

public abstract class SessionHandler
{
	internal virtual Func<BaseSession, PokemonMessage, Task> Delegate =>
		null!;
}

public abstract class SessionHandler<TContext, TMessage> : SessionHandler
	where TContext : BaseSession
	where TMessage : PokemonMessage
{
	internal override Func<BaseSession, PokemonMessage, Task> Delegate =>
		(client, message) => HandleAsync((TContext)client, (TMessage)message);

	protected abstract Task HandleAsync(TContext session, TMessage message);
}