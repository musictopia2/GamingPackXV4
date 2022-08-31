namespace HitTheDeck.Core.Data;
[UseScoreboard]
public partial class HitTheDeckPlayerItem : PlayerSingleHand<HitTheDeckCardInformation>
{
    [ScoreColumn]
    public int PreviousPoints { get; set; }
    [ScoreColumn]
    public int TotalPoints { get; set; }
}