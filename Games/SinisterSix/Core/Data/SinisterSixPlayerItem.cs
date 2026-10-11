namespace SinisterSix.Core.Data;
[UseScoreboard]
public partial class SinisterSixPlayerItem : SimplePlayer
{
    [ScoreColumn]
    public partial int Score { get; set; }
}