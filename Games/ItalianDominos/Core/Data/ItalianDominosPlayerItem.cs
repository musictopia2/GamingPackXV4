namespace ItalianDominos.Core.Data;
[UseScoreboard]
public partial class ItalianDominosPlayerItem : PlayerSingleHand<SimpleDominoInfo>
{
    [ScoreColumn] //does not hurt making as scorecolumn
    public int TotalScore { get; set; }
    [ScoreColumn]
    public bool DrewYet { get; set; }
}