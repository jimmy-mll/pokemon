using Microsoft.Xna.Framework;
using Pokemon.Client.Models.Network;
using Pokemon.Core.Network.Dispatching.Handlers;
using Pokemon.Core.Network.Metadata;
using Pokemon.Protocol.Messages.Spawning;
using Pokemon.Server.Network;

namespace Pokemon.Server.Handlers.Spawning;

public class NewPlayerSpawnRequestHandler : SessionHandler<PokemonServer, PokemonSession, NewPlayerSpawnedRequestMessage>
{
    protected override async Task HandleAsync(PokemonServer server, PokemonSession session, NewPlayerSpawnedRequestMessage message)
    {
        if (string.IsNullOrEmpty(message.Id))
        {
            Console.WriteLine($"The message id received in the {nameof(NewPlayerSpawnedMessage)} was null or empty.");
            return; //TODO: Create an InvalidOperationMessage
        }

        var gamePlayer = new GamePlayer()
        {
            Id = message.Id,
            Position = new Vector2(Random.Shared.NextSingle() * -500f + 500f, Random.Shared.NextSingle() * -250f + 250f)
        };

        var otherPlayers = server.Players.ToList();

        server.Players.Add(gamePlayer);

        await server.BroadcastMessageAsync(new NewPlayerSpawnedMessage()
        {
            SpawnedPlayer = gamePlayer,
            OtherPlayers = otherPlayers,
        });
    }
}