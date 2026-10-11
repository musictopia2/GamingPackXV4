namespace Cribbage.Core.Data;
[UseScoreboard]
public partial class CribbagePlayerItem : PlayerSingleHand<CribbageCard>
{
    [ScoreColumn]
    public partial int ScoreRound { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    [ScoreColumn]
    public partial bool IsSkunk { get; set; } = true;
    [ScoreColumn]
    public partial bool FinishedLooking { get; set; }
    [ScoreColumn]
    public partial int FirstPosition { get; set; }
    [ScoreColumn]
    public partial int SecondPosition { get; set; }
    [ScoreColumn]
    public partial bool ChoseCrib { get; set; }
}