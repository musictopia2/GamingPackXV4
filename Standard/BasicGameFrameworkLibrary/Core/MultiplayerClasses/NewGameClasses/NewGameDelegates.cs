namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.NewGameClasses;
public static class NewGameDelegates
{
    public static Func<RawGameHost, Task>? NewGameHostStep1 { get; set; } //if i redo, try to rename this one.
    public static Func<RawGameClient, Task>? NewGameClientStep1 { get; set; } //if i do redo, will try to remove this one (?)
    public static Action? ReloadOnWPF { get; set; }
}