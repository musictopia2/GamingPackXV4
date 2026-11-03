namespace Uno.Core.Data;
[UseScoreboard]
public partial class UnoPlayerItem : PlayerSingleHand<UnoCardInformation>
{
    [ScoreColumn]
    public partial int TotalPoints { get; set; }
    [ScoreColumn]
    public partial int PreviousPoints { get; set; }
}