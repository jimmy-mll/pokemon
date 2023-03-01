using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Pokemon.Monogame.ECS;
using Pokemon.Monogame.Utils;
using Serilog;

namespace Pokemon.Monogame;

public abstract class AbstractGame : Game
{
	private const string LoggingTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
	
	public GraphicsDeviceManager Graphics { get; }

	public GameScene Scene
	{
		get => _currentScene;
		set
		{
			if (_currentScene == value) return;

			_currentScene?.Unload();

			_currentScene = value;
			_shouldLoadSceneNextFrame = true;

        }
	}
	
	public new IServiceProvider Services { get; private set; }
	
	protected IConfiguration Configuration { get; private set; }

	private bool _shouldLoadSceneNextFrame;
	private GameScene? _currentScene;

	protected AbstractGame(int width, int height, string title)
	{
		Graphics = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferHeight = height,
			PreferredBackBufferWidth = width
		};
		Window.Title = title;
		Window.AllowUserResizing = true;
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		Graphics.ApplyChanges();
	}

	protected sealed override void Initialize()
	{
		Configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", false, true)
			.Build();

		Log.Logger = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.WriteTo.Console(outputTemplate: LoggingTemplate)
			.CreateLogger();

		var services = new ServiceCollection();
		services.AddSingleton(typeof(AbstractGame), this);

		services
			.AddSingleton(Configuration)
			.AddLogging(x => x.ClearProviders().AddSerilog(dispose: true));

		ConfigureServices(services);

		Services = services.BuildServiceProvider();

		InitializeServices();

		TextureUtils.Initialize(GraphicsDevice);

        base.Initialize();
	}

	protected abstract void InitializeServices();

	protected abstract void ConfigureServices(IServiceCollection services);

    protected override void Update(GameTime gameTime)
    {
		if (_shouldLoadSceneNextFrame)
		{
			Scene?.Load();
			_shouldLoadSceneNextFrame = false;
		}

		Scene?.Update(gameTime);

        base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		Scene?.Draw(gameTime);

		base.Draw(gameTime);
	}
}