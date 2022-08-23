namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Data;

public class FirstGameData
{
    public bool Client { get; set; }
    public BasicList<PlayerInfo> PlayerList { get; set; } = new();
    public string NickName { get; set; } = "";
}