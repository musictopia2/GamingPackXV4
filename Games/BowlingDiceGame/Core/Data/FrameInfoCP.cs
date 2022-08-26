namespace BowlingDiceGame.Core.Data;
public class FrameInfoCP
{
    public Dictionary<int, SectionInfoCP> SectionList { get; set; } = new();
    public int Score { get; set; } = -1;
}