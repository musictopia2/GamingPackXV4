namespace YahtzeeHandsDown.Core.Data;
[UseScoreboard]
public partial class YahtzeeHandsDownPlayerItem : PlayerSingleHand<YahtzeeHandsDownCardInformation>
{
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    [ScoreColumn]
    public partial int ScoreRound { get; set; }
    [ScoreColumn]
    public partial string WonLastRound { get; set; } = "";
    public YahtzeeResults Results { get; set; } = new ();
}