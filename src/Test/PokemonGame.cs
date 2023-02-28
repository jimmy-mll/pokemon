using Microsoft.Extensions.DependencyInjection;
using Pokemon.Client.Handlers.Player;
using Pokemon.Core.Extensions;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Factory;
using Pokemon.Core.Network.Transport;
using Pokemon.Engine;
using Pokemon.Protocol.Messages.Player;

namespace Pokemon.Client;

public class PokemonGame : GameEngine
{
    protected override void OnConfiguration(IServiceCollection services)
    {
        services
            .AddSingleton<PlayerHandler>()
            .AddNetwork<PokemonClient>();

        base.OnConfiguration(services);
    }

    protected override void Initialize()
    {
        Services
            .GetRequiredService<IMessageFactory>()
            .Initialize(typeof(PlayerSpawnMessage).Assembly);
        
        Services
            .GetRequiredService<IMessageDispatcher>()
            .InitializeClient(typeof(PokemonGame).Assembly);
        
        _ = NetworkManager.ConnectAsync();
        
        base.Initialize();
    }
}