namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;
public abstract class SimpleSave
{
    public string GameID { get; set; } = ""; //for now, no need for even the part for canprivatesave.  this may not even care.
    public void GetNewID()
    {
        GameID = Guid.NewGuid().ToString();
    }
}