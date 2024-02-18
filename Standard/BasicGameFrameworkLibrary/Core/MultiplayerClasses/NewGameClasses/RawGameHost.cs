namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.NewGameClasses;
public sealed class RawGameHost
{
    public BasicList<RawPlayer> Players { get; set; } = [];
    public string GameName { get; set; } = "";
    public int WhoStarts { get; set; }
    public bool Multiplayer { get; set; } //needs to know if its even multiplayer.
}