using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pokemon.Client.Network;
using Pokemon.Core.Extensions;
using Pokemon.Core.Network.Dispatching;
using Pokemon.Core.Network.Factory;
using Pokemon.Core.Network.Framing;
using Pokemon.Core.Network.Options;
using Pokemon.Monogame;
using Pokemon.Protocol.Messages.Authentication;

namespace Pokemon.Client.Services.Game;

public sealed class PokemonGame : AbstractGame
{
	private SpriteBatch _spriteBatch;

	public PokemonGame() : base(1280, 720, "Pokémon")
	{
	}
	
	protected override void ConfigureServices(IServiceCollection services)
	{
		services
			.AddSingleton(this)
			.AddSingleton<IMessageParser, MessageParser>()
			.AddSingleton<IMessageFactory, MessageFactory>()
			.AddSingleton<IMessageDispatcher, MessageDispatcher>()
			.AddSingleton<PokemonClient>()
			.Configure<ClientOptions>(Configuration.GetRequiredSection("Network"));
	}

	protected override void InitializeServices()
	{
		var messageFactory = Services.GetRequiredService<IMessageFactory>();
		var messageDispatcher = Services.GetRequiredService<IMessageDispatcher>();
		var client = Services.GetRequiredService<PokemonClient>();

		messageFactory.Initialize(typeof(HelloConnectMessage).Assembly);
		messageDispatcher.InitializeClient(typeof(PokemonGame).Assembly);
		client.ConnectAsync().FireAndForget();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);
	}
	
	protected override void UnloadContent()
	{
		_spriteBatch.Dispose();
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		base.Draw(gameTime);
	}
}