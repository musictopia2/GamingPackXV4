namespace LottoDominos.Core.Data;
[UseScoreboard]
public partial class LottoDominosPlayerItem : SimplePlayer
{
    [ScoreColumn]
    public int NumberChosen { get; set; } = -1;
    [ScoreColumn]
    public int NumberWon { get; set; }
}