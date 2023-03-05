using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Network.Transport;

namespace Pokemon.Core.Network.Dispatching.Handlers;

public abstract class SessionHandler
{
	internal virtual Func<IBaseServer, BaseSession, PokemonMessage, Task> Delegate =>
		null!;
}

public abstract class SessionHandler<TServer, TSession, TMessage> : SessionHandler
	where TServer : BaseServer<TSession>
	where TSession : BaseSession
	where TMessage : PokemonMessage
{
	internal override Func<IBaseServer, BaseSession, PokemonMessage, Task> Delegate =>
		(server, session, message) => HandleAsync((TServer)server, (TSession)session, (TMessage)message);

	protected abstract Task HandleAsync(TServer server, TSession session, TMessage message);
}