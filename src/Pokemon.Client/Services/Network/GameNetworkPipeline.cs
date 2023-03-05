using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Pokemon.Client.Notifications;

namespace Pokemon.Client.Services.Network;

public class GameNetworkPipeline : IGameNetworkPipeline
{
    //private TaskCompletionSource<PokemonMessage> _tcs = null;

    public delegate Task GameNetworkPipelineEventHandler(EventArgs e);
    private readonly Dictionary<NotificationType, GameNetworkPipelineEventHandler> _handlers;

    public GameNetworkPipeline()
    {
        _handlers = new Dictionary<NotificationType, GameNetworkPipelineEventHandler>();
    }

    public void RegisterNotification<TEventArgs>(NotificationType type, Func<TEventArgs, Task> handler) where TEventArgs : EventArgs
    {
        if (_handlers.ContainsKey(type))
            throw new Exception($"A handler with the same key is already existing. ({type})");

        _handlers.Add(type, (args) => handler((TEventArgs)args));
    }

    public void UnregisterNotification(NotificationType type)
        => _handlers.Remove(type);

    public async Task NotifyAsync<TEventArgs>(NotificationType type, TEventArgs e) where TEventArgs : EventArgs
    {
        if (!_handlers.TryGetValue(type, out var handler))
        {
            Console.WriteLine($"No handler registered for this notification type ({type}).");
            return;
        }

        await handler.Invoke(e);
    }
}