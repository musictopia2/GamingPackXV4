namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.NewGameClasses;
public sealed class RawPlayer
{
    public int Id { get; set; }
    public string NickName { get; set; } = "";
    public EnumPlayerCategory PlayerCategory { get; set; }
    public bool IsHost { get; set; }
}