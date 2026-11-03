namespace Checkers.Core.Data;
[SingletonGame]
public class CheckersSaveInfo : BasicSavedGameClass<CheckersPlayerItem>, IMappable, ISaveInfo
{
    public int SpaceHighlighted
    {
        get
        {
            return GameBoardGraphicsCP.SpaceSelected;
        }
        set
        {
            if (ForcedToMove)
            {
                //code to run.  don't want to change lots of code to make this static.
            }
            GameBoardGraphicsCP.SpaceSelected = value;
        }
    }
    public EnumGameStatus GameStatus { get; set; }
    public bool ForcedToMove { get; set; }
}