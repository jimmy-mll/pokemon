using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pokemon.Client.Models.Network;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Options;
using Pokemon.Core.Network.Transport;
using Pokemon.Protocol.Messages.Spawning;

namespace Pokemon.Server.Network;

public sealed class PokemonServer : BaseServer<PokemonSession>
{
	public List<GamePlayer> Players { get; private set; }

	public PokemonServer(IMessageParser messageParser, IMessageDispatcher messageDispatcher, ILogger<BaseServer<PokemonSession>> logger, IOptions<ServerOptions> options)
		: base(messageParser, messageDispatcher, logger, options)
	{
		Players = new List<GamePlayer>();
	}

	protected override PokemonSession CreateSession(Socket socket, IMessageParser messageParser, IMessageDispatcher messageDispatcher) =>
		new(socket, this, messageParser, messageDispatcher);

    protected override async Task OnSessionDisconnectedAsync(PokemonSession session)
    {
        var player = GetPlayerBySession(session);

        if (player is null)
            return;

        Players.Remove(player);

        await this.BroadcastMessageAsync(new UnspawnedPlayerMessage(session.SessionId));

        await base.OnSessionDisconnectedAsync(session);
    }

    private GamePlayer? GetPlayerBySession(BaseSession session)
    {
        return Players.FirstOrDefault(x => x.Id == session.SessionId);
    }
}