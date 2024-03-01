namespace MonopolyDicedGame.Core.Data;
[UseScoreboard]
public partial class MonopolyDicedGamePlayerItem : SimplePlayer
{
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}