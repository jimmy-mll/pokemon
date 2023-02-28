using Microsoft.Extensions.DependencyInjection;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Factory;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Transport;

namespace Pokemon.Core.Extensions;

public static class DependencyInjectionExtensions
{
    public static void AddNetwork<TPeer>(this IServiceCollection services) where TPeer : class, INetworkPeer
    {
        services.AddSingleton<IMessageParser, MessageParser>();
        services.AddSingleton<IMessageFactory, MessageFactory>();
        services.AddSingleton<IMessageDispatcher, MessageDispatcher>();

        services.AddSingleton<TPeer>();
    }
}
