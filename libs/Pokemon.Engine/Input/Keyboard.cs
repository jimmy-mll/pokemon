using Raylib_CsLo;

namespace Pokemon.Engine.Input;

public static class Keyboard
{
    public static bool IsKeyDown(KeyboardKey key) =>
        Raylib.IsKeyDown(key);
    
    public static bool IsKeyUp(KeyboardKey key) =>
        Raylib.IsKeyUp(key);
    
    public static bool IsKeyPressed(KeyboardKey key) =>
        Raylib.IsKeyPressed(key);
    
    public static bool IsKeyReleased(KeyboardKey key) =>
        Raylib.IsKeyReleased(key);

    public static bool IsKeyComboDown(params KeyboardKey[] keys) =>
        keys.All(IsKeyDown);
    
    public static bool IsKeyComboUp(params KeyboardKey[] keys) =>
        keys.All(IsKeyUp);
    
    public static bool IsKeyComboPressed(params KeyboardKey[] keys) =>
        keys.All(IsKeyPressed);
    
    public static bool IsKeyComboReleased(params KeyboardKey[] keys) =>
        keys.All(IsKeyReleased);
}