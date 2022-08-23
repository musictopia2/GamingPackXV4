namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;

public abstract class SimpleSave
{
    public bool CanPrivateSave { get; set; }
    public string GameID { get; set; } = "";
    public void GetNewID()
    {
        GameID = Guid.NewGuid().ToString();
    }
}