using System;

public static class PartySystemEvents
{
    public static event Action OnStatsUpdated;
    
    public static void RaiseStatsUpdated() => OnStatsUpdated?.Invoke();
}
