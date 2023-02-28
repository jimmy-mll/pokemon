using Microsoft.Extensions.DependencyInjection;
using Pokemon.Core.Extensions;
using Pokemon.Core.Network.Transport;
using System.Net;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Factory;
using Pokemon.Protocol.Messages.Player;
using Pokemon.Server.Handlers.Player;

var servicesCollection = new ServiceCollection();
servicesCollection
    .AddSingleton<PlayerHandler>()
    .AddNetwork<PokemonServer>();

var services = servicesCollection.BuildServiceProvider();

services.GetRequiredService<IMessageFactory>().Initialize(typeof(PlayerSpawnMessage).Assembly);
services.GetRequiredService<IMessageDispatcher>().InitializeServer(typeof(PlayerHandler).Assembly);

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
    await session.SendAsync(new PlayerSpawnRequestMessage()).ConfigureAwait(false);
}

Console.ReadKey();