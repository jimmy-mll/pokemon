using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Options;
using Pokemon.Core.Network.Transport;
using Pokemon.Protocol.Messages.Authentication;

namespace Pokemon.Client.Network;

public sealed class PokemonClient : BaseClient
{
	public string Username { get; set; } = string.Empty;

	public string Password { get; set; } = string.Empty;

	public PokemonClient(IMessageParser messageParser, IMessageDispatcher messageDispatcher, ILogger<BaseClient> logger, IOptions<ClientOptions> options)
		: base(messageParser, messageDispatcher, logger, options)
	{
	}

	protected override async ValueTask OnConnectedAsync()
	{
		Username = "Aerafal";
		var encryptedPassword = Convert.ToBase64String(MD5.HashData(Encoding.UTF8.GetBytes(Password)));

		await base.OnConnectedAsync();
		await SendAsync(new IdentificationRequestMessage(Username, encryptedPassword));
	}
}