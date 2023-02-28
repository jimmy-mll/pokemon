using System.Net.Sockets;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Transport;

namespace Pokemon.Server.Network;

public sealed class PokemonSession : BaseSession
{
	public PokemonSession(Socket socket, IMessageParser messageParser, IMessageDispatcher messageDispatcher) : base(socket, messageParser, messageDispatcher)
	{
	}
}