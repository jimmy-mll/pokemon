using System;

namespace Pokemon.Client.Notifications.Spawning;

public class OtherClientUnspawnedEventArgs : EventArgs
{
    public string UnspawnedPlayerId { get; private set; }

    public OtherClientUnspawnedEventArgs(string unspawnedPlayerId)
    {
        UnspawnedPlayerId = unspawnedPlayerId;
    }
}
