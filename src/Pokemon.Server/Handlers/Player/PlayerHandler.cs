using System.Text.Json;
using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Network.Transport;
using Pokemon.Protocol.Messages.Player;

namespace Pokemon.Server.Handlers.Player;

public sealed class PlayerHandler
{
	[MessageHandler]
	public async Task HandlePlayerSpawnRequestAsync(PokemonSession session, PlayerSpawnMessage message)
	{
		Console.WriteLine(JsonSerializer.Serialize(message));
	}
}