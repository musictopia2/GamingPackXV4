namespace ThinkTwice.Core.Data;
[UseScoreboard]
public partial class ThinkTwicePlayerItem : SimplePlayer
{
    [ScoreColumn]
    public int ScoreRound { get; set; }
    [ScoreColumn]
    public int ScoreGame { get; set; }
}