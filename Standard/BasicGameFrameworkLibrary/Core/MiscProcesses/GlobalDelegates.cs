namespace BasicGameFrameworkLibrary.Core.MiscProcesses;

public static class GlobalDelegates
{
    public static Action<IEventAggregator>? RefreshSubscriptions { get; set; }
}