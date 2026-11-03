namespace DominosRegular.Core.Data;
[UseScoreboard]
public partial class DominosRegularPlayerItem : PlayerSingleHand<SimpleDominoInfo>
{
    [ScoreColumn] //does not hurt making as scorecolumn
    public partial int TotalScore { get; set; }
    public bool NoPlay { get; set; }
}