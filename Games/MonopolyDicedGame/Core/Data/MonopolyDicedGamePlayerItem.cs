namespace MonopolyDicedGame.Core.Data;
[UseScoreboard]
public partial class MonopolyDicedGamePlayerItem : SimplePlayer
{
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}