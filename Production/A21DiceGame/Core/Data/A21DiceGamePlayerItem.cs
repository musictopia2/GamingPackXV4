namespace A21DiceGame.Core.Data;
[UseScoreboard]
public partial class A21DiceGamePlayerItem : SimplePlayer
{
    [ScoreColumn]
    public partial bool IsFaceOff { get; set; }
    [ScoreColumn]
    public partial int Score { get; set; }
    [ScoreColumn]
    public partial int NumberOfRolls { get; set; }
}