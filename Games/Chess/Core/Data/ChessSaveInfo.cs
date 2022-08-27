namespace Chess.Core.Data;
[SingletonGame]
public class ChessSaveInfo : BasicSavedGameClass<ChessPlayerItem>, IMappable, ISaveInfo
{
    public int SpaceHighlighted
    {
        get
        {
            return GameBoardGraphicsCP.SpaceSelected;
        }
        set
        {
            if (GameStatus == EnumGameStatus.None)
            {
                //needed so i can still access from reference without warnings.
            }
            GameBoardGraphicsCP.SpaceSelected = value;
        }
    }
    public EnumGameStatus GameStatus { get; set; }
    public PreviousMove? PossibleMove { get; set; }
    public PreviousMove PreviousMove { get; set; } = new();
}