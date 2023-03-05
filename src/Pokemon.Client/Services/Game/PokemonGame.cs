using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Client.Components.Scenes;
using Pokemon.Client.Handlers.Authentication;
using Pokemon.Client.Handlers.Spawning;
using Pokemon.Client.Network;
using Pokemon.Client.Notifications;
using Pokemon.Client.Notifications.Authentication;
using Pokemon.Client.Services.Network;
using Pokemon.Core.Extensions;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Factory;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Options;
using Pokemon.Monogame;
using Pokemon.Monogame.Services.Keyboard;
using Pokemon.Monogame.Services.Scenes;
using Pokemon.Monogame.Services.Textures;
using Pokemon.Protocol.Messages.Authentication;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Pokemon.Client.Services.Game;

public sealed class PokemonGame : AbstractGame
{
	private TaskCompletionSource<AuthenticationResultEventArgs> _tcs;

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
			.AddScoped<NewPlayerSpawnedHandler>()
			.AddScoped<PlayerUnspawnedHandler>()
			.AddScoped<AuthenticationFailedHandler>()
			.AddScoped<AuthenticationSuccessHandler>()
            .AddSingleton<ITextureService, TextureService>()
			.AddScoped<ISceneService, SceneService>()
			.AddSingleton<IKeyboardService, KeyboardService>()
			.AddSingleton<IGameNetworkPipeline, GameNetworkPipeline>()
			.AddSingleton<MainScene>()
			.Configure<ClientOptions>(Configuration.GetRequiredSection("Network"));
	}

	protected override void InitializeServices()
	{
		var messageFactory = Services.GetRequiredService<IMessageFactory>();
		var messageDispatcher = Services.GetRequiredService<IMessageDispatcher>();
		
		messageFactory.Initialize(typeof(IdentificationRequestMessage).Assembly);
		messageDispatcher.InitializeClient(typeof(PokemonGame).Assembly);

		_tcs = new TaskCompletionSource<AuthenticationResultEventArgs>();

		var pipeline = Services.GetRequiredService<IGameNetworkPipeline>();
        pipeline.RegisterNotification<AuthenticationResultEventArgs>(NotificationType.AuthenticationResultNotification, e =>
        {
            _tcs.TrySetResult(e);
			return Task.CompletedTask;
        });
    }

	protected override void LoadContent()
	{
        var scene = Services.GetRequiredService<MainScene>();

        var networkClient = Services.GetRequiredService<PokemonClient>();
        networkClient.Username = "user";
        networkClient.Password = "password";
        networkClient.ConnectAsync().FireAndForget();

        var authenticationResult = _tcs.Task.GetAwaiter().GetResult();
		if (!authenticationResult.IsSuccess)
            return;

        //We have to get the scene before to prevent the possible message/packet loss.
        Scene = scene; 

		var textureManager = Services.GetRequiredService<ITextureService>();
		textureManager.AddTexture(GameSprites.Pikachu, Content.Load<Texture2D>("pikachu"));
		textureManager.AddTexture(GameSprites.PlayerWalk, Content.Load<Texture2D>("walk"));
		textureManager.AddTexture(GameSprites.PlayerRun, Content.Load<Texture2D>("run"));

        Services.GetRequiredService<IKeyboardService>().LoadMappings();

        base.LoadContent();
	}
}