using Microsoft.Extensions.DependencyInjection;
using System;

namespace Pokemon.Engine;

public static class ServicesManager
{
    public static object? GetService(Type type)
        => _services.GetService(type);

    public static T GetService<T>()
        => _services.GetService<T>();

    public static object GetRequiredService(Type type)
        => _services.GetRequiredService(type);

    public static T GetRequiredService<T>()
        => _services.GetRequiredService<T>();

    private static IServiceProvider _services => GameEngine.Instance.Services;
}
