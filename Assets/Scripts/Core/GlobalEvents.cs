using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEvents
{
    public static event Action<ObsidianPlayerStats> OnPlayerStatsChanged;
    public static void InvokeOnPlayerStatsChanged(ObsidianPlayerStats stats)
    {
        OnPlayerStatsChanged?.Invoke(stats);
    }
}
