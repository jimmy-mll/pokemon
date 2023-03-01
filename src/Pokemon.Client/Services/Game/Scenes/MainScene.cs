using System;
using Arch.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.Monogame;
using Pokemon.Monogame.ECS;
using Pokemon.Monogame.ECS.Components.Entities;
using Pokemon.Monogame.ECS.Components.Renderers;
using Pokemon.Monogame.Services.Keyboard;

namespace Pokemon.Client.Services.Game.Scenes;

public class MainScene : GameScene
{
	private const float MoveSpeed = 50f;
	
	private readonly IKeyboardService _keyboardService;
	
	public MainScene(AbstractGame game, IKeyboardService keyboardService) : base(game)
	{
		_keyboardService = keyboardService;
	}

	protected override void OnLoad()
	{
		Services.GetRequiredService<IKeyboardService>().LoadMappings();
		
		var renderer = new SpriteSheetRenderer(GameSprites.PlayerWalk, Color.White, new SpriteSheet(new Vector2(128), new Vector2(32)));

		World.Create<IRenderer, Position, Scale>(renderer, new Position(50, 50), new Scale(1f, 1f));

		base.OnLoad();
	}

	protected override void OnUpdate(GameTime gameTime)
	{
		var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

		var query = new QueryDescription()
			.WithAll<IRenderer, Position, Scale>();
		
		World.Query(query, (ref IRenderer renderer, ref Position position, ref Scale scale) =>
		{
			foreach (var pressedKey in Keyboard.GetState().GetPressedKeys())
			{
				var mapping = _keyboardService.GetMappingForKey(pressedKey);

				switch (mapping)
				{
					case KeyboardMappings.Up:
						position.Y -= MoveSpeed * dt;
						break;
					
					case KeyboardMappings.Down:
						position.Y += MoveSpeed * dt;
						break;
					
					case KeyboardMappings.Left:
						position.X -= MoveSpeed * dt;
						break;
					
					case KeyboardMappings.Right:
						position.X += MoveSpeed * dt;
						break;
					
					case KeyboardMappings.None:
					default:
						continue;
				}
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