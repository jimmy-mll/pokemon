using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Serilog;

namespace Pokemon.Monogame;

public abstract class AbstractGame : Game
{
	private const string LoggingTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
	
	protected GraphicsDeviceManager Graphics { get; }
	
	protected new IServiceProvider Services { get; private set; }
	
	protected IConfiguration Configuration { get; private set; }

	protected AbstractGame(int width, int height, string title)
	{
		Graphics = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferHeight = height,
			PreferredBackBufferWidth = width,
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
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

		Log.Logger = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.WriteTo.Console(outputTemplate: LoggingTemplate)
			.CreateLogger();
		
		var services = new ServiceCollection();
		
		services
			.AddSingleton(Configuration)
			.AddLogging(x => x.ClearProviders().AddSerilog(dispose: true));

		ConfigureServices(services);

		Services = services.BuildServiceProvider();
		
		InitializeServices();
		
		base.Initialize();
	}

	protected abstract void ConfigureServices(IServiceCollection services);

	protected abstract void InitializeServices();
}