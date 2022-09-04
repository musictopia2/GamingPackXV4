namespace BasicGameFrameworkLibrary.Blazor.Helpers;
public static class LoaderGlobalClass
{
    public static Action? BackToMainDelegate { get; set; }
    public static Func<IJSRuntime, Task>? LoadSettingsAsync { get; set; }
    public static Func<IJSRuntime, Task>? SaveSettingsAsync { get; set; }
    
    //i think it belongs here since its a loader.
    //this means needs one more library which only maui or wpf would have for transferring to desktop.

}