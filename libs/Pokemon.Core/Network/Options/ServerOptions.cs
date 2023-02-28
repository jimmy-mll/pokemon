namespace Pokemon.Core.Network.Options;

public class ServerOptions : ClientOptions
{
	public required int MaxConnections { get; set; }
	
	public required int MaxConnectionsPerAddress { get; set; }
}