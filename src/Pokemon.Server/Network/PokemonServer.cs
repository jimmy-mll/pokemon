using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Options;
using Pokemon.Core.Network.Transport;

namespace Pokemon.Server.Network;

public sealed class PokemonServer : BaseServer<PokemonSession>
{
	public PokemonServer(IMessageParser messageParser, IMessageDispatcher messageDispatcher, ILogger<BaseServer<PokemonSession>> logger, IOptions<ServerOptions> options)
		: base(messageParser, messageDispatcher, logger, options)
	{
	}

	protected override PokemonSession CreateSession(Socket socket, IMessageParser messageParser, IMessageDispatcher messageDispatcher) =>
		new(socket, messageParser, messageDispatcher);
}