namespace A21DiceGame.Core.Data;
[UseScoreboard]
public partial class A21DiceGamePlayerItem : SimplePlayer
{
    [ScoreColumn]
    public bool IsFaceOff { get; set; }
    [ScoreColumn]
    public int Score { get; set; }
    [ScoreColumn]
    public int NumberOfRolls { get; set; }
}