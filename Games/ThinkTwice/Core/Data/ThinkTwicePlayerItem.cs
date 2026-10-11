namespace ThinkTwice.Core.Data;
[UseScoreboard]
public partial class ThinkTwicePlayerItem : SimplePlayer
{
    [ScoreColumn]
    public partial int ScoreRound { get; set; }
    [ScoreColumn]
    public partial int ScoreGame { get; set; }
}