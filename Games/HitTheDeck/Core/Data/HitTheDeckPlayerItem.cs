namespace HitTheDeck.Core.Data;
[UseScoreboard]
public partial class HitTheDeckPlayerItem : PlayerSingleHand<HitTheDeckCardInformation>
{
    [ScoreColumn]
    public partial int PreviousPoints { get; set; }
    [ScoreColumn]
    public partial int TotalPoints { get; set; }
}