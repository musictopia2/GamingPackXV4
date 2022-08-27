namespace ConnectTheDots.Core.Data;
[UseScoreboard]
public partial class ConnectTheDotsPlayerItem : PlayerBoardGame<EnumColorChoice>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
    [ScoreColumn]
    public int Score { get; set; }
}