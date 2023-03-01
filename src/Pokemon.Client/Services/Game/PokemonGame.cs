using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Client.Handlers.Authentication;
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
			.AddSingleton<AuthenticationFailedHandler>()
			.AddSingleton<AuthenticationSuccessHandler>()
			.AddSingleton<ITextureService, TextureService>()
			.AddSingleton<ISceneService, SceneService>()
			.AddSingleton<MainScene>()
			.Configure<ClientOptions>(Configuration.GetRequiredSection("Network"));
	}

	protected override void InitializeServices()
	{
		var messageFactory = Services.GetRequiredService<IMessageFactory>();
		var messageDispatcher = Services.GetRequiredService<IMessageDispatcher>();
		var client = Services.GetRequiredService<PokemonClient>();

		messageFactory.Initialize(typeof(IdentificationRequestMessage).Assembly);
		messageDispatcher.InitializeClient(typeof(PokemonGame).Assembly);
		client.ConnectAsync().FireAndForget();
	}

	protected override void LoadContent()
	{
		Scene = Services.GetRequiredService<MainScene>();

		var textureManager = Services.GetRequiredService<ITextureService>();
		textureManager.AddTexture(GameSprites.Pikachu, Content.Load<Texture2D>("pikachu"));

		base.LoadContent();
	}
}