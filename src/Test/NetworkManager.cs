using Pokemon.Core.Network.Transport;
using Pokemon.Engine;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Pokemon.Client;

public static class NetworkManager
{
    private static readonly IPEndPoint _remoteEP = new(IPAddress.Parse("127.0.0.1"), 9981);

    public static async Task ConnectAsync()
    {
        var client = ServicesManager.GetRequiredService<PokemonClient>();
        client.Connected += Client_Connected;

        await client.ConnectAsync(_remoteEP);
    }

    private static ValueTask Client_Connected()
    {
        Console.WriteLine("Connected to the server.");

        return ValueTask.CompletedTask;
    }
}