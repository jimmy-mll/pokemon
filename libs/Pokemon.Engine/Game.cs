using System.Diagnostics;
using System.Reflection;
using Raylib_CsLo;

namespace Pokemon.Engine;

public abstract class Game : IDisposable
{
	private float _lastDt;

	public void Dispose()
	{
	}

	protected virtual void Update(float dt)
	{
	}

	protected virtual void Draw()
	{
	}

	public void Run()
	{
		Raylib.InitWindow(1280, 720, Assembly.GetExecutingAssembly().FullName ?? "Game");

		var sw = new Stopwatch();

		while (!Raylib.WindowShouldClose())
		{
			sw.Start();

			Update(_lastDt);

			Raylib.BeginDrawing();

			Draw();

			Raylib.EndDrawing();

			sw.Stop();
			_lastDt = (float)sw.Elapsed.TotalSeconds;
		}

		Raylib.CloseWindow();
	}
}