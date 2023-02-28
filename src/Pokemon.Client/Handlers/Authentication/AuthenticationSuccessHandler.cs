using System.Threading.Tasks;
using Pokemon.Client.Network;
using Pokemon.Core.Network.Dispatching.Handlers;
using Pokemon.Protocol.Messages.Authentication;

namespace Pokemon.Client.Handlers.Authentication;

public sealed class AuthenticationSuccessHandler : ClientHandler<PokemonClient, IdentificationSuccessMessage>
{
	protected override Task HandleAsync(PokemonClient client, IdentificationSuccessMessage message)
	{
		return Task.CompletedTask;
	}
}