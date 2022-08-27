namespace Checkers.Core.Data;
public class CheckersPlayerItem : PlayerBoardGame<EnumColorChoice>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
    public BasicList<PlayerSpace> CurrentPieceList { get; set; } = new();
    public BasicList<PlayerSpace> StartPieceList { get; set; } = new();
    public bool PossibleTie { get; set; }
}