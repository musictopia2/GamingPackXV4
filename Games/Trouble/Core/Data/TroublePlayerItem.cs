namespace Trouble.Core.Data;
[UseScoreboard]
public partial class TroublePlayerItem : PlayerBoardGame<EnumColorChoice>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
    public BasicList<int> PieceList { get; set; } = new();
}