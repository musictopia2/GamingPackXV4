namespace XPuzzle.Core.Data;
[SingletonGame]
public class XPuzzleSaveInfo : IMappable, ISaveInfo
{
    public Vector PreviousOpen { get; set; }

    public XPuzzleCollection SpaceList { get; set; } = new();
}