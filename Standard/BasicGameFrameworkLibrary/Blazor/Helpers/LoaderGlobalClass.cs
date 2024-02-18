﻿namespace BasicGameFrameworkLibrary.Blazor.Helpers;
public static class LoaderGlobalClass
{
    public static Action? BackToMainDelegate { get; set; }
    public static Func<IJSRuntime, Task>? LoadSettingsAsync { get; set; }
    public static Func<IJSRuntime, Task>? SaveSettingsAsync { get; set; }
    //no longer necessary because you usually 
    //public static Action<string>? ChangeLatestGame { get; set; }
}