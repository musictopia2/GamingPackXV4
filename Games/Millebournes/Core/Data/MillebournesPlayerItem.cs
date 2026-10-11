namespace Millebournes.Core.Data;
[UseScoreboard]
public partial class MillebournesPlayerItem : PlayerSingleHand<MillebournesCardInformation>
{
    [ScoreColumn]
    public partial int Team { get; set; }
    [ScoreColumn]
    public partial bool OtherTurn { get; set; }
    [ScoreColumn]
    public partial int Miles { get; set; }
    [ScoreColumn]
    public partial int OtherPoints { get; set; }
    [ScoreColumn]
    public partial int TotalPoints { get; set; }
    [ScoreColumn]
    public partial int Number200s { get; set; }
}