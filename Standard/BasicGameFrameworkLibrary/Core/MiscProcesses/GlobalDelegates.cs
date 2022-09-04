namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public static class GlobalDelegates
{
    //if when recompiling if i find a game that uses RefreshSubscriptions, then put back in again.

    //public static Action<IEventAggregator>? RefreshSubscriptions { get; set; }
    //try to not even do the refreshsubscriptions.  if i am wrong, rethink.

    public static Func<string, Task>? TransferToDesktop { get; set; }
}