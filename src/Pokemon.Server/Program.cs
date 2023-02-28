using Microsoft.Extensions.DependencyInjection;
using Pokemon.Core.Extensions;
using Pokemon.Core.Network.Metadata.STC;
using Pokemon.Core.Network.Transport;
using System.Net;

var servicesCollection = new ServiceCollection();
servicesCollection.AddNetwork<PokemonServer>();

var services = servicesCollection.BuildServiceProvider();
var server = services.GetRequiredService<PokemonServer>();

server.SessionConnected += Server_SessionConnected;
server.SessionDisconnected += Server_SessionDisconnected;

await server.StartAsync(new IPEndPoint(IPAddress.Any, 9981));

Task Server_SessionDisconnected(PokemonSession session)
{
    Console.WriteLine($"{session.RemoteEndPoint} disconnected.");

    return Task.CompletedTask;
}

async Task Server_SessionConnected(PokemonSession session)
{
    Console.WriteLine($"New client connected: {session.RemoteEndPoint}");
    await session.SendAsync(new NewPlayerSpawnedMessage(0, 200, 200)).ConfigureAwait(false);
}