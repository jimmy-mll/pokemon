using System.Threading.Tasks;
using Pokemon.Client.Network;
using Pokemon.Core.Network.Dispatching.Handlers;
using Pokemon.Protocol.Messages.Authentication;

namespace Pokemon.Client.Handlers.Authentication;

public sealed class AuthenticationFailedHandler : ClientHandler<PokemonClient, IdentificationFailedMessage>
{
	protected override Task HandleAsync(PokemonClient client, IdentificationFailedMessage message)
	{
		return Task.CompletedTask;
	}
}