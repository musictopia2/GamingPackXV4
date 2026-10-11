namespace DeadDie96.Core.Data;
[UseScoreboard]
public partial class DeadDie96PlayerItem : SimplePlayer
{
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}