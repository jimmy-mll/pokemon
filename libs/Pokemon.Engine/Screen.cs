namespace Pokemon.Engine;

public static class Screen
{
    public static int Width => Core.Graphics.PreferredBackBufferWidth;
    public static int Height => Core.Graphics.PreferredBackBufferHeight;

    private static GameEngine Core => GameEngine.Instance;
}