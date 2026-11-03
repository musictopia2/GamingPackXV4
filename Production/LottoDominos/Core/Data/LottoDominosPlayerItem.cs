namespace LottoDominos.Core.Data;
[UseScoreboard]
public partial class LottoDominosPlayerItem : SimplePlayer
{
    [ScoreColumn]
    public partial int NumberChosen { get; set; } = -1;
    [ScoreColumn]
    public partial int NumberWon { get; set; }
}