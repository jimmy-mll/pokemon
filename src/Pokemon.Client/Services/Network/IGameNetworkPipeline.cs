using Pokemon.Client.Notifications;
using System;
using System.Threading.Tasks;

namespace Pokemon.Client.Services.Network;

public interface IGameNetworkPipeline
{
    void RegisterNotification<TEventArgs>(NotificationType type, Func<TEventArgs, Task> handler) where TEventArgs : EventArgs;
    void UnregisterNotification(NotificationType type);
    Task NotifyAsync<TEventArgs>(NotificationType type, TEventArgs e) where TEventArgs : EventArgs;
}
