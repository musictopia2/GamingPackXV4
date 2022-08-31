namespace Uno.Core.Data;
[UseScoreboard]
public partial class UnoPlayerItem : PlayerSingleHand<UnoCardInformation>
{
    [ScoreColumn]
    public int TotalPoints { get; set; }
    [ScoreColumn]
    public int PreviousPoints { get; set; }
}