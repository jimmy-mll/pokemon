using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Pokemon.Engine.Exceptions;
using System;

namespace Pokemon.Engine;

public abstract class GameEngine : Game
{
    public static GameEngine Instance => _lazy?.Value ?? throw new Exception("You're trying to access an instance of a class that wasn't instantied.");

    public new IServiceProvider Services { get; }
    public GraphicsDeviceManager Graphics => _graphics;

    public GameEngine()
    {
        if (_lazy != null)
            throw new MultipleInstanceException(nameof(GameEngine));

        _lazy = new Lazy<GameEngine>(() => this, true);
        _graphics = new GraphicsDeviceManager(this);

        IsMouseVisible = true;

        var services = new ServiceCollection();
        this.OnConfiguration(services);

        Services = services.BuildServiceProvider();
    }

    protected virtual void OnConfiguration(IServiceCollection services) { }

    private static Lazy<GameEngine> _lazy;
    private readonly GraphicsDeviceManager _graphics;
}
