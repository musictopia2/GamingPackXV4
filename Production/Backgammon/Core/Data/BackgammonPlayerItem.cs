namespace Backgammon.Core.Data;
[UseScoreboard]
public partial class BackgammonPlayerItem : PlayerBoardGame<EnumColorChoice>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
    public BackgammonPlayerDetails? StartTurnData { get; set; }
    public BackgammonPlayerDetails? CurrentTurnData { get; set; }
}