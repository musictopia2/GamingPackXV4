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
}