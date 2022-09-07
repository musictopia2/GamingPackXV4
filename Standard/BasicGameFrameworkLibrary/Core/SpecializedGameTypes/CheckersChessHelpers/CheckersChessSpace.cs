namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.CheckersChessHelpers;
public abstract class CheckersChessSpace<E>
    where E : IFastEnumColorSimple
{
    public int MainIndex { get; set; }
    public int ReversedIndex { get; set; }
    public int Column { get; set; }
    public int Row { get; set; }
    public bool WasReversed { get; set; }
    public string Color { get; set; } = "";
    protected abstract EnumCheckerChessGame GetGame();
    public RectangleF ThisRect { get; set; }
    public PointF GetLocation()
    {
        if (_thisGame == EnumCheckerChessGame.Checkers)
        {
            return new(ThisRect.Location.X, ThisRect.Location.Y);
        }
        return new(ThisRect.Location.X + 3, ThisRect.Location.Y + 3);
    }
    public SizeF GetSize()
    {
        return ThisRect.Size;
    }
    public abstract void ClearSpace();
    public abstract CheckerChessPieceCP<E>? GetGamePiece();

    private readonly EnumCheckerChessGame _thisGame;
    public CheckersChessSpace()
    {
        _thisGame = GetGame();
    }
}