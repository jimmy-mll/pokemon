using System.Numerics;
using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Network.Transport;
using System.Threading.Tasks;
using Pokemon.Protocol.Messages.Player;

namespace Pokemon.Client.Handlers.Player;

public sealed class PlayerHandler
{
    [MessageHandler]
    public async Task HandlePlayerSpawnAsync(PokemonClient client, PlayerSpawnRequestMessage message)
    {
        await client.SendAsync(new PlayerSpawnMessage(new Vector2(544f, 44.5f)));
    }
}
