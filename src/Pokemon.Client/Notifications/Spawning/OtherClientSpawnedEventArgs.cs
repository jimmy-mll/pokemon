using Pokemon.Client.Models.Network;
using System.Collections.Generic;
using System;

namespace Pokemon.Client.Notifications.Spawning;

public class OtherClientSpawnedEventArgs : EventArgs
{
    public GamePlayer SpawnedPlayer { get; }

    public OtherClientSpawnedEventArgs(GamePlayer spawnedPlayer)
    {
        SpawnedPlayer = spawnedPlayer;
    }
}
