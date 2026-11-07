namespace BasicGameFrameworkLibrary.Blazor.Helpers;
public sealed class ReloadGameRequest
{
    public string GameName { get; set; } = "";
    public bool IsClient { get; set; }
    public bool IsLocalGame { get; set; }
}