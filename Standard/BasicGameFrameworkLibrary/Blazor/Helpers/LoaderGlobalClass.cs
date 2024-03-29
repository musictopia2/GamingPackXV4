﻿namespace BasicGameFrameworkLibrary.Blazor.Helpers;
public static class LoaderGlobalClass
{
    public static Action? BackToMainDelegate { get; set; }
    public static Func<IJSRuntime, Task>? LoadSettingsAsync { get; set; }
    public static Func<IJSRuntime, Task>? SaveSettingsAsync { get; set; }
    public static Func<Task>? RefreshForNewGameDelegate { get; set; } //if nobody is handling it, then will 
}