namespace SinisterSix.Core.Data;
[UseScoreboard]
public partial class SinisterSixPlayerItem : SimplePlayer
{
    [ScoreColumn]
    public int Score { get; set; }
}