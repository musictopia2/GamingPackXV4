namespace BasicGameFrameworkLibrary.Blazor.Helpers;
public static class LoaderGlobalClass
{
    public static bool ShowFullScreenButton { get; set; }
    public static Func<Task>? BackToMainDelegate { get; set; }
    public static bool ChoosingNextGameAsHost { get; set; }
    public static string AutoHostGameName { get; set; } = "";
    public static int NextGameHumanCount { get; set; }
    public static Func<string, Task>? WaitForNextGameAsync { get; set; }
    public static Func<IJSRuntime, Task>? LoadSettingsAsync { get; set; }
    public static Func<IJSRuntime, Task>? SaveSettingsAsync { get; set; }


    public static Func<Task>? ReloadGameAsync { get; set; }
    public static ReloadGameRequest? PendingReload { get; set; }

}