using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pokemon.Client.Network;
using Pokemon.Client.Services.Game.Scenes;
using Pokemon.Core.Extensions;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Factory;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Options;
using Pokemon.Monogame;
using Pokemon.Monogame.Services.Scenes;
using Pokemon.Monogame.Services.Textures;
using Pokemon.Protocol.Messages.Authentication;

namespace Pokemon.Client.Services.Game;

public sealed class PokemonGame : AbstractGame
{
	public PokemonGame() : base(1280, 720, "Pokémon")
	{
	}
	
	protected override void ConfigureServices(IServiceCollection services)
	{
		services
			.AddSingleton<IMessageParser, MessageParser>()
			.AddSingleton<IMessageFactory, MessageFactory>()
			.AddSingleton<IMessageDispatcher, MessageDispatcher>()
			.AddSingleton<PokemonClient>()
			.AddSingleton<ITextureManagerServices, TextureManagerServices>()
			.AddScoped<ISceneManagerServices, SceneManagerServices>()
			.Configure<ClientOptions>(Configuration.GetRequiredSection("Network"));

		this.ConfigureScenes(services);
	}

	private void ConfigureScenes(IServiceCollection services)
	{
		services.AddSingleton<MainScene>();
	}

	protected override void InitializeServices()
	{
		var messageFactory = Services.GetRequiredService<IMessageFactory>();
		var messageDispatcher = Services.GetRequiredService<IMessageDispatcher>();
		var client = Services.GetRequiredService<PokemonClient>();

		messageFactory.Initialize(typeof(HelloConnectMessage).Assembly);
		messageDispatcher.InitializeClient(typeof(PokemonGame).Assembly);
		//client.ConnectAsync().FireAndForget();
	}

    protected override void LoadContent()
    {
		Scene = Services.GetRequiredService<MainScene>();

		var textureManager = Services.GetRequiredService<ITextureManagerServices>();
		textureManager.AddTexture(GameSprites.Pikachu, Content.Load<Texture2D>("pikachu"));

        base.LoadContent();
    }
}