namespace Cribbage.Core.Data;
[UseScoreboard]
public partial class CribbagePlayerItem : PlayerSingleHand<CribbageCard>
{
    [ScoreColumn]
    public int ScoreRound { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
    [ScoreColumn]
    public bool IsSkunk { get; set; } = true;
    [ScoreColumn]
    public bool FinishedLooking { get; set; }
    [ScoreColumn]
    public int FirstPosition { get; set; }
    [ScoreColumn]
    public int SecondPosition { get; set; }
    [ScoreColumn]
    public bool ChoseCrib { get; set; }
}