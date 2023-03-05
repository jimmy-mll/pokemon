using System;
using System.Threading.Tasks;
using Pokemon.Client.Network;
using Pokemon.Client.Notifications;
using Pokemon.Client.Notifications.Authentication;
using Pokemon.Client.Services.Network;
using Pokemon.Core.Network.Dispatching.Handlers;
using Pokemon.Protocol.Messages.Authentication;
using Serilog;

namespace Pokemon.Client.Handlers.Authentication;

public sealed class AuthenticationFailedHandler : ClientHandler<PokemonClient, IdentificationFailedMessage>
{
	private readonly IGameNetworkPipeline _pipeline;

	public AuthenticationFailedHandler(IGameNetworkPipeline pipeline)
		=> _pipeline = pipeline;

	protected override async Task HandleAsync(PokemonClient client, IdentificationFailedMessage message)
	{
		await _pipeline.NotifyAsync(NotificationType.AuthenticationResultNotification, new AuthenticationResultEventArgs(false, message.Reason));
	}
}