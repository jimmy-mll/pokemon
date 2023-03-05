using Pokemon.Client.Network;
using Pokemon.Client.Notifications;
using Pokemon.Client.Notifications.Spawning;
using Pokemon.Client.Services.Network;
using Pokemon.Core.Network.Dispatching.Handlers;
using Pokemon.Protocol.Messages.Spawning;
using System.Threading.Tasks;

namespace Pokemon.Client.Handlers.Spawning;

public class NewPlayerSpawnedHandler : ClientHandler<PokemonClient, NewPlayerSpawnedMessage>
{
    private readonly IGameNetworkPipeline _pipeline;

    public NewPlayerSpawnedHandler(IGameNetworkPipeline pipeline)
        => _pipeline = pipeline;

    protected override async Task HandleAsync(PokemonClient client, NewPlayerSpawnedMessage message)
    {
        if (client.Id == message.SpawnedPlayer.Id)
        {
            await _pipeline.NotifyAsync(NotificationType.CurrentClientSpawnedNotification, new CurrentClientSpawnedEventArgs(message.SpawnedPlayer, message.OtherPlayers));
        }
        else
        {
            await _pipeline.NotifyAsync(NotificationType.OtherClientSpawnedNotification, new OtherClientSpawnedEventArgs(message.SpawnedPlayer));
        }
    }
}