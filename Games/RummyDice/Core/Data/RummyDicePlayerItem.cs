namespace RummyDice.Core.Data;
[UseScoreboard]
public partial class RummyDicePlayerItem : SimplePlayer
{
    [ScoreColumn]
    public int ScoreRound { get; set; }
    [ScoreColumn]
    public int ScoreGame { get; set; }
    [ScoreColumn]
    public int Phase { get; set; }
    public int HowManyRepeats { get; set; }
    public int CurrentRepeats { get; set; } //can influence the chances of getting what you need.
}