using System.Security.Cryptography;
using System.Text;
using Pokemon.Core.Network.Dispatching.Handlers;
using Pokemon.Protocol.Enums;
using Pokemon.Protocol.Messages.Authentication;
using Pokemon.Server.Network;

namespace Pokemon.Server.Handlers.Authentication;

public sealed class AuthenticationHandler : SessionHandler<PokemonServer, PokemonSession, IdentificationRequestMessage>
{
	protected override async Task HandleAsync(PokemonServer server, PokemonSession session, IdentificationRequestMessage message)
	{
		var decryptedPassword = message.Password;//Marche pas: Encoding.UTF8.GetString(MD5.HashData(Convert.FromBase64String(message.Password)));

		const string expectedPassword = "password";

		// Exemple
		if (!string.Equals(expectedPassword, decryptedPassword))
		{
			await session.SendAsync(new IdentificationFailedMessage(IdentificationFailureReasons.InvalidCredentials));
			return;
		}

		await session.SendAsync(new IdentificationSuccessMessage(session.SessionId));
	}
}