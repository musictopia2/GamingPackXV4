namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.CheckersChessHelpers;
public abstract class CheckersChessSpace<E>
    where E : IFastEnumColorSimple
{
    public int MainIndex { get; set; }
    public int ReversedIndex { get; set; }
    public int Column { get; set; }
    public int Row { get; set; }
    public bool WasReversed { get; set; } // in some cases, it needs to know this.
    public string Color { get; set; } = "";
    protected abstract EnumCheckerChessGame GetGame();
    public RectangleF ThisRect { get; set; } // this is the rectangle which is needed in order to draw things on it.
    public PointF GetLocation()
    {
        if (_thisGame == EnumCheckerChessGame.Checkers)
        {
            return new(ThisRect.Location.X, ThisRect.Location.Y);
        }
        return new(ThisRect.Location.X + 3, ThisRect.Location.Y + 3); // can rethink if necessary (?)
    }
    public SizeF GetSize()
    {
        return ThisRect.Size; //hopefully no need for 32 by 32 because of the new svg chess pieces.
    }
    public abstract void ClearSpace(); // whatever needs to be done to clear the space.
    public abstract CheckerChessPieceCP<E>? GetGamePiece(); //has to be protected because blazor needs the info from it.

    private readonly EnumCheckerChessGame _thisGame;
    public CheckersChessSpace()
    {
        _thisGame = GetGame();
    }
}