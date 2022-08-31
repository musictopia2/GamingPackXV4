namespace YahtzeeHandsDown.Core.Data;
[UseScoreboard]
public partial class YahtzeeHandsDownPlayerItem : PlayerSingleHand<YahtzeeHandsDownCardInformation>
{
    [ScoreColumn]
    public int TotalScore { get; set; }
    [ScoreColumn]
    public int ScoreRound { get; set; }
    [ScoreColumn]
    public string WonLastRound { get; set; } = "";
    public YahtzeeResults Results { get; set; } = new ();
}