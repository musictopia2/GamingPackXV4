namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.NewGameClasses;
public static class NewGameDelegates
{
    public static Func<RawGameHost, Task>? NewGameHostStep1 { get; set; }
    public static Func<RawGameClient, Task>? NewGameClientStep1 { get; set; }
}