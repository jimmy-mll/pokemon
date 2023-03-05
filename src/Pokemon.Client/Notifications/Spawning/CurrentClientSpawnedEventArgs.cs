using Pokemon.Client.Models.Network;
using System;
using System.Collections.Generic;

namespace Pokemon.Client.Notifications.Spawning;

public class CurrentClientSpawnedEventArgs : EventArgs
{
    public GamePlayer SpawnedPlayer { get; }
    public List<GamePlayer> OtherPlayers { get; }

    public CurrentClientSpawnedEventArgs(GamePlayer spawnedPlayer, List<GamePlayer> otherPlayers)
    {
        SpawnedPlayer = spawnedPlayer;
        OtherPlayers = otherPlayers;
    }
}
