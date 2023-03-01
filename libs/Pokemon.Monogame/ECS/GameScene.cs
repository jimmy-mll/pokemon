using System;
using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Monogame.ECS.Components;
using Pokemon.Monogame.ECS.Components.Entities;
using Pokemon.Monogame.ECS.Components.Renderers;

namespace Pokemon.Monogame.ECS;

public abstract class GameScene
{
	public World World { get; private set; }

	public AbstractGame Game { get; }
	public SpriteBatch SpriteBatch { get; private set; }
	public ContentManager Content => Game.Content;
	public IServiceProvider Services => Game.Services;
	public GraphicsDeviceManager Graphics => Game.Graphics;
	public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

	public GameScene(AbstractGame game) =>
		Game = game;

	/// <summary>
	///     Called when the scene is loaded
	/// </summary>
	protected virtual void OnLoad()
	{
	}

	/// <summary>
	///     Called when the scene is unloaded
	/// </summary>
	protected virtual void OnUnload()
	{
	}

	/// <summary>
	///     Called when the scene is updated
	/// </summary>
	protected virtual void OnUpdate(GameTime gameTime)
	{
	}

	/// <summary>
	///     Called when the scene is rendered
	/// </summary>
	protected virtual void OnDraw(GameTime gameTime)
	{
	}

	public void Load()
	{
		World = World.Create();
		SpriteBatch = new SpriteBatch(GraphicsDevice);

		OnLoad();
	}

	public void Unload()
	{
		OnUnload();
		World.Destroy(World);
	}

	public void Update(GameTime gameTime)
	{
		OnUpdate(gameTime);
	}

	public void Draw(GameTime gameTime)
	{
		OnDraw(gameTime);

		SpriteBatch.Begin();

		var queryDesc = new QueryDescription().WithAny<IRenderer, Position, Scale>();
		World.Query(in queryDesc, (ref IRenderer renderer, ref Position position, ref Scale scale) => { renderer.Render(this, SpriteBatch, position, scale); });

		SpriteBatch.End();
	}
}