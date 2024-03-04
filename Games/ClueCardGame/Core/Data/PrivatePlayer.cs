namespace ClueCardGame.Core.Data;
public class PrivatePlayer
{
    public int Id { get; set; } //to link up to computer player.
    public Dictionary<int, DetectiveInfo> ComputerDetectiveNoteBook { get; set; } = [];
    public BasicList<GivenInfo> CluesGiven = [];

}