namespace MonopolyCardGame.Core.Data;
public class PrivateModel : IMappable
{
    public Dictionary<int, DetectiveInfo> PersonalNotebook = [];
    public BasicList<PrivatePlayer> ComputerData { get; set; } = []; //so the computer can make smarter decisions.
    public bool StartAccusation { get; set; } //this is if you are starting to make accusation.  other players don't need to know about this.
}