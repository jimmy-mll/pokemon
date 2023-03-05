using System.Threading.Tasks;
using Pokemon.Client.Network;
using Pokemon.Client.Notifications;
using Pokemon.Client.Notifications.Authentication;
using Pokemon.Client.Services.Network;
using Pokemon.Core.Network.Dispatching.Handlers;
using Pokemon.Protocol.Messages.Authentication;
using Pokemon.Protocol.Messages.Spawning;

namespace Pokemon.Client.Handlers.Authentication;

public sealed class AuthenticationSuccessHandler : ClientHandler<PokemonClient, IdentificationSuccessMessage>
{
    private readonly IGameNetworkPipeline _pipeline;

    public AuthenticationSuccessHandler(IGameNetworkPipeline pipeline)
        => _pipeline = pipeline;

	protected override async Task HandleAsync(PokemonClient client, IdentificationSuccessMessage message)
	{
        client.Id = message.Id;

        await client.SendAsync(new NewPlayerSpawnedRequestMessage(message.Id));
        await _pipeline.NotifyAsync(NotificationType.AuthenticationResultNotification, new AuthenticationResultEventArgs(true));
    }
}