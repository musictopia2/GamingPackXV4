namespace BowlingDiceGame.Core.Data;
public class BowlingDiceGamePlayerItem : SimplePlayer
{
    public int TotalScore { get; set; }
    public Dictionary<int, FrameInfoCP> FrameList { get; set; } = new();
}