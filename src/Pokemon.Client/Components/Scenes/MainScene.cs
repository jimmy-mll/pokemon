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
	private const float MoveSpeed = 150f;

	private readonly IKeyboardService _keyboardService;
	private Dictionary<string, AnimationData> _animationBindings;
	private AnimationController _animationController;

	private PlayerDirection _playerDirection;

	public MainScene(AbstractGame game, IKeyboardService keyboardService) : base(game) =>
		_keyboardService = keyboardService;

	protected override void OnLoad()
	{
		Services.GetRequiredService<IKeyboardService>().LoadMappings();

		var spriteSheet = new SpriteSheet(GameSprites.PlayerWalk, new Vector2(128), new Vector2(32));

		var idleDown = new AnimationData(1, true, spriteSheet, 0);
		var idleUp = new AnimationData(1, true, spriteSheet, 4);
		var idleLeft = new AnimationData(1, true, spriteSheet, 8);
		var idleRight = new AnimationData(1, true, spriteSheet, 12);
		var walkDown = new AnimationData(8, true, spriteSheet, 0, 1, 2, 3);
		var walkUp = new AnimationData(8, true, spriteSheet, 4, 5, 6, 7);
		var walkLeft = new AnimationData(8, true, spriteSheet, 8, 9, 10, 11);
		var walkRight = new AnimationData(8, true, spriteSheet, 12, 13, 14, 15);

		_animationBindings = new Dictionary<string, AnimationData>
		{
			{ "IdleDown", idleDown },
			{ "IdleUp", idleUp },
			{ "IdleLeft", idleLeft },
			{ "IdleRight", idleRight },
			{ "WalkDown", walkDown },
			{ "WalkUp", walkUp },
			{ "WalkLeft", walkLeft },
			{ "WalkRight", walkRight }
		};

		var spriteRenderer = new SpriteRenderer();
		_animationController = new AnimationController(spriteRenderer, _animationBindings["WalkDown"]);

		World.Create<IRenderer, AnimationController, Position, Scale>(spriteRenderer, _animationController, default, new Scale(2.5f));

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
			var input = Vector2.Zero;

			foreach (var pressedKey in Keyboard.GetState().GetPressedKeys())
			{
				var mapping = _keyboardService.GetMappingForKey(pressedKey);

				switch (mapping)
				{
					case KeyboardMappings.Up:
						position.Y -= MoveSpeed * dt;
						_playerDirection = PlayerDirection.Up;
						input.Y = -1;
						break;

					case KeyboardMappings.Down:
						position.Y += MoveSpeed * dt;
						_playerDirection = PlayerDirection.Down;
						input.Y = 1;
						break;

					case KeyboardMappings.Left:
						position.X -= MoveSpeed * dt;
						_playerDirection = PlayerDirection.Left;
						input.X = -1;
						break;

					case KeyboardMappings.Right:
						position.X += MoveSpeed * dt;
						_playerDirection = PlayerDirection.Right;
						input.X = 1;
						break;

					case KeyboardMappings.None:
					default:
						continue;
				}
			}

			if (input == Vector2.Zero)
				switch (_playerDirection)
				{
					case PlayerDirection.Left:
						_animationController.Play(_animationBindings["IdleLeft"]);
						break;
					case PlayerDirection.Right:
						_animationController.Play(_animationBindings["IdleRight"]);
						break;
					case PlayerDirection.Up:
						_animationController.Play(_animationBindings["IdleUp"]);
						break;
					case PlayerDirection.Down:
						_animationController.Play(_animationBindings["IdleDown"]);
						break;
					default:
						_animationController.Play(_animationBindings["IdleDown"]);
						break;
				}
			else
				switch (_playerDirection)
				{
					case PlayerDirection.Left:
						_animationController.Play(_animationBindings["WalkLeft"]);
						break;
					case PlayerDirection.Right:
						_animationController.Play(_animationBindings["WalkRight"]);
						break;
					case PlayerDirection.Up:
						_animationController.Play(_animationBindings["WalkUp"]);
						break;
					case PlayerDirection.Down:
						_animationController.Play(_animationBindings["WalkDown"]);
						break;
					default:
						_animationController.Play(_animationBindings["WalkDown"]);
						break;
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