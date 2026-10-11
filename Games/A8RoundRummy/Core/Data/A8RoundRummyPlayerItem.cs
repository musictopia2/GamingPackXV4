namespace A8RoundRummy.Core.Data;
[UseScoreboard]
public partial class A8RoundRummyPlayerItem : PlayerSingleHand<A8RoundRummyCardInformation>
{
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}