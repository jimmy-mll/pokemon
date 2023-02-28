using Microsoft.Extensions.DependencyInjection;
using Pokemon.Core.Extensions;
using Pokemon.Core.Network.Transport;
using Pokemon.Engine;

namespace Pokemon.Client;

public class PokemonGame : GameEngine
{
    protected override void OnConfiguration(IServiceCollection services)
    {
        services.AddNetwork<PokemonClient>();

        base.OnConfiguration(services);
    }

    protected override async void Initialize()
    {
        await NetworkManager.ConnectAsync();



        base.Initialize();
    }
}