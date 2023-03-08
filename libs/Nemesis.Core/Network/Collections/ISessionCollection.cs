using Nemesis.Core.Network.Metadata;
using Nemesis.Core.Network.Transport;

namespace Nemesis.Core.Network.Collections;

public interface ISessionCollection<in TSession>
	where TSession : BaseSession
{
	void Add(TSession session);

	void Remove(TSession session);

	Task BroadcastMessageAsync(PokemonMessage message);
}