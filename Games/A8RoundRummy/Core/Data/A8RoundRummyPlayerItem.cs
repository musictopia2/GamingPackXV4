namespace A8RoundRummy.Core.Data;
[UseScoreboard]
public partial class A8RoundRummyPlayerItem : PlayerSingleHand<A8RoundRummyCardInformation>
{
    [ScoreColumn]
    public int TotalScore { get; set; }
}