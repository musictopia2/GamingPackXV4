namespace ItalianDominos.Core.Data;
[UseScoreboard]
public partial class ItalianDominosPlayerItem : PlayerSingleHand<SimpleDominoInfo>
{
    [ScoreColumn] //does not hurt making as scorecolumn
    public partial int TotalScore { get; set; }
    [ScoreColumn]
    public partial bool DrewYet { get; set; }
}