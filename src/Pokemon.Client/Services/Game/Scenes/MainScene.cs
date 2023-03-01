using System;
using Arch.Core;
using Microsoft.Xna.Framework;
using Pokemon.Monogame;
using Pokemon.Monogame.ECS;
using Pokemon.Monogame.ECS.Components;
using Pokemon.Monogame.ECS.Components.Renderers;
using Pokemon.Monogame.Services.Textures;
using Pokemon.Monogame.Utils;

namespace Pokemon.Client.Services.Game.Scenes;

public class MainScene : GameScene
{
	private static readonly Color[] _colors = { Color.White, Color.Red, Color.Blue, Color.Yellow, Color.Magenta, Color.Cyan, Color.Aqua, Color.Pink };
	private readonly ITextureManagerServices _textureManager;
	private Vector2 _pikachuRealSize;

	public MainScene(AbstractGame game, ITextureManagerServices textureManager) : base(game) =>
		_textureManager = textureManager;

	protected override void OnLoad()
	{
		var padding = 50;
		var scale = new Scale(0.05f, 0.05f);

		_pikachuRealSize = TextureUtils.GetTextureSize(_textureManager.GetTexture(GameSprites.Pikachu)) * scale;

		for (var i = 0; i < 1500; i++)
		{
			var renderer = new SpriteRenderer(GameSprites.Pikachu, _colors[Random.Shared.Next(_colors.Length)]);

			var posX = Random.Shared.Next(padding, Graphics.PreferredBackBufferWidth - padding);
			var posY = Random.Shared.Next(padding, Graphics.PreferredBackBufferHeight - padding);
			var position = new Position(posX, posY);

			var force = Random.Shared.NextSingle() * 50f + 25f;
			var angle = Random.Shared.NextSingle() * MathF.PI * 2.0f;
			var velocity = new Velocity(MathF.Cos(angle) * force, MathF.Sin(angle) * force);

			World.Create<IRenderer, Scale, Position, Velocity>(renderer, scale, position, velocity);
		}

		base.OnLoad();
	}

	protected override void OnUpdate(GameTime gameTime)
	{
		var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

		var query = new QueryDescription().WithAll<IRenderer, Position, Velocity, Scale>();
		World.Query(query, (ref IRenderer renderer, ref Position position, ref Velocity velocity, ref Scale scale) =>
		{
			position.X += velocity.X * dt;
			position.Y += velocity.Y * dt;

			if (position.X - _pikachuRealSize.X * 0.5f < 0.0f || position.X + _pikachuRealSize.X * 0.5f > Graphics.PreferredBackBufferWidth)
			{
				velocity.X = -velocity.X;
				renderer.Color = _colors[Random.Shared.Next(_colors.Length)];
			}

			if (position.Y - _pikachuRealSize.Y * 0.5f < 0.0f || position.Y + _pikachuRealSize.Y * 0.5f > Graphics.PreferredBackBufferHeight)
			{
				velocity.Y = -velocity.Y;
				renderer.Color = _colors[Random.Shared.Next(_colors.Length)];
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