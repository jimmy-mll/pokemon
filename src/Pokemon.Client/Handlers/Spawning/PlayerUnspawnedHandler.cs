using Pokemon.Client.Network;
using Pokemon.Client.Notifications;
using Pokemon.Client.Notifications.Spawning;
using Pokemon.Client.Services.Network;
using Pokemon.Core.Network.Dispatching.Handlers;
using Pokemon.Protocol.Messages.Spawning;
using System.Threading.Tasks;

namespace Pokemon.Client.Handlers.Spawning;

public class PlayerUnspawnedHandler : ClientHandler<PokemonClient, UnspawnedPlayerMessage>
{
    private readonly IGameNetworkPipeline _pipeline;

    public PlayerUnspawnedHandler(IGameNetworkPipeline pipeline)
        => _pipeline = pipeline;

    protected override async Task HandleAsync(PokemonClient client, UnspawnedPlayerMessage message)
    {
        await _pipeline.NotifyAsync(NotificationType.OtherClientUnspawnedNotification, new OtherClientUnspawnedEventArgs(message.UnspawnedPlayerId));
    }
}
