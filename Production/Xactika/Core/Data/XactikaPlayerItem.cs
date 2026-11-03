namespace Xactika.Core.Data;
[UseScoreboard]
public partial class XactikaPlayerItem : PlayerTrick<EnumShapes, XactikaCardInformation>
{
    [ScoreColumn]
    public partial int BidAmount { get; set; }
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}