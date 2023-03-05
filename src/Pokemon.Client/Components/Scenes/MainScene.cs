using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arch.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.Client.Components.Entities;
using Pokemon.Client.Network;
using Pokemon.Client.Notifications;
using Pokemon.Client.Notifications.Authentication;
using Pokemon.Client.Notifications.Spawning;
using Pokemon.Client.Services.Game;
using Pokemon.Client.Services.Network;
using Pokemon.Core.Extensions;
using Pokemon.Monogame;
using Pokemon.Monogame.ECS;
using Pokemon.Monogame.ECS.Components.Entities;
using Pokemon.Monogame.ECS.Components.Interfaces;
using Pokemon.Monogame.ECS.Components.Renderers;
using Pokemon.Monogame.Models;
using Pokemon.Monogame.Services.Keyboard;

namespace Pokemon.Client.Components.Scenes;


public class MainScene : GameScene
{
	private readonly IKeyboardService _keyboardService;
	private readonly IGameNetworkPipeline _pipeline;

    private TaskCompletionSource<bool> _tcs;

    public MainScene(AbstractGame game, IKeyboardService keyboardService, IGameNetworkPipeline pipeline) : base(game)
	{
		_pipeline = pipeline;
		_keyboardService = keyboardService;
		_tcs = new TaskCompletionSource<bool>();

		_pipeline.RegisterNotification<CurrentClientSpawnedEventArgs>(NotificationType.CurrentClientSpawnedNotification,async e =>
		{
			await _tcs.Task;

			this.SpawnPlayer(true, e.SpawnedPlayer.Id, e.SpawnedPlayer.Position);

			for (int i = 0; i < e.OtherPlayers.Count; i++)
			{
				var item = e.OtherPlayers[i];
				this.SpawnPlayer(false, item.Id, item.Position);
			}
		});

		_pipeline.RegisterNotification<OtherClientSpawnedEventArgs>(NotificationType.OtherClientSpawnedNotification, async e =>
		{
			await _tcs.Task;

			this.SpawnPlayer(false, e.SpawnedPlayer.Id, e.SpawnedPlayer.Position);
		});

		_pipeline.RegisterNotification<OtherClientUnspawnedEventArgs>(NotificationType.OtherClientUnspawnedNotification, async e =>
		{
			await _tcs.Task;

			this.UnspawnPlayer(e.UnspawnedPlayerId);
		});
	}

    protected override void OnLoad()
	{
		_tcs.TrySetResult(true); //Wait to load the game before spawning the players.

		base.OnLoad();
	}

	private void UnspawnPlayer(string playerId)
	{
		World.Query(new QueryDescription().WithAll<NetworkPlayerComponent>(), (in Entity entity, ref NetworkPlayerComponent netPlayerComponent) =>
		{
			if (netPlayerComponent.Id == playerId)
				World.Destroy(in entity);
		});
	}

	private void SpawnPlayer(bool currentPlayer, string id, Position position)
	{
        var spriteRenderer = new SpriteRenderer();
		var animationController = new AnimationController(spriteRenderer, GameAnimations.PlayerIdle["Down"]);

		if (!currentPlayer) World.Create<IRenderer, AnimationController, Position, Scale, NetworkPlayerComponent>(spriteRenderer, animationController, position, new Scale(2.5f, 2.5f), new NetworkPlayerComponent(id, currentPlayer));
		else World.Create<IRenderer, AnimationController, Position, Scale, NetworkPlayerComponent, CharacterController>(spriteRenderer, animationController, position, new Scale(2.5f, 2.5f), new NetworkPlayerComponent(id, currentPlayer), new CharacterController());
    }

	protected override void OnUpdate(GameTime gameTime)
	{
		var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

		var queryDesc = new QueryDescription().WithAll<AnimationController>();

		World.Query(queryDesc, (ref AnimationController animationController) =>
		{
            animationController?.Update(gameTime);
        });

		queryDesc = new QueryDescription()
			.WithAll<AnimationController, Position, CharacterController>();

		World.Query(queryDesc, (ref CharacterController controller, ref Position position,  ref AnimationController animationController) =>
		{
			controller.Update(ref position, animationController, _keyboardService, gameTime);
		});

		base.OnUpdate(gameTime);
	}

	protected override void OnDraw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		base.OnDraw(gameTime);
	}
}