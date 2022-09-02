namespace Xactika.Core.Data;
[UseScoreboard]
public partial class XactikaPlayerItem : PlayerTrick<EnumShapes, XactikaCardInformation>
{
    [ScoreColumn]
    public int BidAmount { get; set; }
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}