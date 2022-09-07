namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public static class GlobalDelegates
{
    public static Func<TransferAutoResumeModel, Task>? TransferToDesktop { get; set; }
}