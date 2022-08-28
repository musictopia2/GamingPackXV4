namespace DeadDie96.Core.Data;
[UseScoreboard]
public partial class DeadDie96PlayerItem : SimplePlayer
{
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}