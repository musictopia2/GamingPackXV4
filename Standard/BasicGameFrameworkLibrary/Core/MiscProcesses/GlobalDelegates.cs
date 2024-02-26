namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public static class GlobalDelegates
{
    public static Func<TransferAutoResumeModel, Task>? TransferToDesktop { get; set; }
    public static Func<string, Task>? DeletePrivateGameNewRound { get; set; } //this is for the client receiving
    public static Func<string, Task>? DeleteOldPrivateGames { get; set; } //this means somebody needs to do something here.
}