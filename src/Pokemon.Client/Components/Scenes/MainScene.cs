using System.Collections.Generic;
using Arch.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.Client.Services.Game;
using Pokemon.Monogame;
using Pokemon.Monogame.ECS;
using Pokemon.Monogame.ECS.Components.Entities;
using Pokemon.Monogame.ECS.Components.Interfaces;
using Pokemon.Monogame.ECS.Components.Renderers;
using Pokemon.Monogame.Models;
using Pokemon.Monogame.Services.Keyboard;

namespace Pokemon.Client.Components.Scenes;

public enum PlayerDirection
{
	Left,
	Right,
	Up,
	Down
}

public class MainScene : GameScene
{
	private const float WalkSpeed = 150f;
	private const float RunSpeed = 250f;

	private PlayerDirection _playerDirection;
	private AnimationController _animationController;

	private PlayerDirection _playerDirection;

	public MainScene(AbstractGame game, IKeyboardService keyboardService) : base(game) =>
		_keyboardService = keyboardService;

	protected override void OnLoad()
	{
		Services.GetRequiredService<IKeyboardService>().LoadMappings();

		/*
		 * Avant d'utiliser les animations group
		 * 
				var idleDown = new Animation(1, true, spriteSheet, 0);
				var idleUp = new Animation(1, true, spriteSheet, 4);
				var idleLeft = new Animation(1, true, sprit(eSheet, 8);
				var idleRight = new Animation(1, true, spriteSheet, 12);
				var walkDown = new Animation(8, true, spriteSheet, 0, 1, 2, 3);
				var walkUp = new Animation(8, true, spriteSheet, 4, 5, 6, 7);
				var walkLeft = new Animation(8, true, spriteSheet, 8, 9, 10, 11);
				var walkRight = new Animation(8, true, spriteSheet, 12, 13, 14, 15);

				_animationBindings = new Dictionary<string, Animation>()
				{
					{ "IdleDown", idleDown },
					{ "IdleUp", idleUp },
					{ "IdleLeft", idleLeft },
					{ "IdleRight", idleRight },
					{ "WalkDown", walkDown },
					{ "WalkUp", walkUp },
					{ "WalkLeft", walkLeft },
					{ "WalkRight", walkRight },
				};
		*/

		var spriteRenderer = new SpriteRenderer();

		_playerDirection = PlayerDirection.Down;
		_animationController = new AnimationController(spriteRenderer, GameAnimations.PlayerIdle["Down"]);
        
		World.Create<IRenderer, AnimationController, Position, Scale>(spriteRenderer, _animationController, new(), new Scale(2.5f, 2.5f));

		base.OnLoad();
	}

	protected override void OnUpdate(GameTime gameTime)
	{
		var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

		var queryDesc = new QueryDescription().WithAll<AnimationController>();

		World.Query(queryDesc, (ref AnimationController animationController) => { animationController.Update(gameTime); });

		queryDesc = new QueryDescription()
			.WithAll<IRenderer, Position, Scale>();

		World.Query(queryDesc, (ref IRenderer _, ref Position position, ref Scale _) =>
		{
			bool isRunning = false;

			Vector2 input = Vector2.Zero;
			var moveSpeed = WalkSpeed;

			foreach (var pressedKey in Keyboard.GetState().GetPressedKeys())
			{
				var mapping = _keyboardService.GetMappingForKey(pressedKey);

				switch (mapping)
				{
					case KeyboardMappings.Up:
						_playerDirection = PlayerDirection.Up;
						input.Y = -1;
						break;

					case KeyboardMappings.Down:
                        _playerDirection = PlayerDirection.Down;
                        input.Y = 1;
                        break;
					
					case KeyboardMappings.Left:
						_playerDirection = PlayerDirection.Left;
						input.X = -1;
						break;

					case KeyboardMappings.Right:
						_playerDirection = PlayerDirection.Right;
						input.X = 1;
						break;

					case KeyboardMappings.Run:
						moveSpeed = RunSpeed;
						isRunning = true;
						break;

					case KeyboardMappings.None:
					default:
						continue;
				}
			}

			position += input * moveSpeed * dt;

			if (input == Vector2.Zero) _animationController.Play(GameAnimations.PlayerIdle[_playerDirection.ToString()]);
			else
			{
				if (isRunning) _animationController.Play(GameAnimations.PlayerRun[_playerDirection.ToString()]);
                else _animationController.Play(GameAnimations.PlayerWalk[_playerDirection.ToString()]);
            }
		});

		base.OnUpdate(gameTime);
	}

	protected override void OnDraw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		base.OnDraw(gameTime);
	}
}