using System.Collections.Concurrent;
using Nemesis.Core.Network.Metadata;
using Nemesis.Core.Network.Transport;

namespace Nemesis.Core.Network.Collections;

public abstract class SessionCollection<TSession> : ISessionCollection<TSession>
	where TSession : BaseSession
{
	private readonly ConcurrentDictionary<string, TSession> _sessions;

	protected SessionCollection() =>
		_sessions = new ConcurrentDictionary<string, TSession>(StringComparer.Ordinal);

	public void Add(TSession session) =>
		_sessions.TryAdd(session.SessionId, session);

	public void Remove(TSession session) =>
		_sessions.TryRemove(session.SessionId, out _);

	public Task BroadcastMessageAsync(PokemonMessage message) =>
		Parallel.ForEachAsync(_sessions.Values, async (session, _) => await session.SendAsync(message).ConfigureAwait(false));
}