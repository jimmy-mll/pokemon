using System;
using Microsoft.Extensions.DependencyInjection;
using Pokemon.Monogame.ECS;

namespace Pokemon.Monogame.Services.Scenes;

public class SceneManagerServices : ISceneManagerServices
{
	private readonly AbstractGame _game;
	private readonly IServiceProvider _serviceProvider;

	public SceneManagerServices(AbstractGame game, IServiceProvider serviceProvider)
	{
		_game = game;
		_serviceProvider = serviceProvider;
	}

	public GameScene GetCurrentScene()
		=> _game.Scene;

	public TScene GetScene<TScene>() where TScene : GameScene
		=> _serviceProvider.GetRequiredService<TScene>();

	public void SetScene<TScene>() where TScene : GameScene
	{
		var scene = _serviceProvider.GetRequiredService<TScene>();
		_game.Scene = scene;
	}
}