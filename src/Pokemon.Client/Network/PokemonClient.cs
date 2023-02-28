using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Options;
using Pokemon.Core.Network.Transport;

namespace Pokemon.Client.Network;

public sealed class PokemonClient : BaseClient
{
	public PokemonClient(IMessageParser messageParser, IMessageDispatcher messageDispatcher, ILogger<BaseClient> logger, IOptions<ClientOptions> options) 
		: base(messageParser, messageDispatcher, logger, options) { }
}