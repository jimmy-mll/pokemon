using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Network.Transport;

namespace Pokemon.Core.Network.Dispatching.Handlers;

public abstract class ClientHandler
{
	internal virtual Func<BaseClient, PokemonMessage, Task> Delegate =>
		null!;
}

public abstract class ClientHandler<TContext, TMessage> : ClientHandler
	where TContext : BaseClient
	where TMessage : PokemonMessage
{
	internal override Func<BaseClient, PokemonMessage, Task> Delegate =>
		(client, message) => HandleAsync((TContext)client, (TMessage)message);

	protected abstract Task HandleAsync(TContext client, TMessage message);
}