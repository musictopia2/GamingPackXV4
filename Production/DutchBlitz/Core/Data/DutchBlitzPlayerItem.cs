namespace DutchBlitz.Core.Data;
[UseScoreboard]
public partial class DutchBlitzPlayerItem : PlayerSingleHand<DutchBlitzCardInformation>
{
    [ScoreColumn]
    public partial int StockLeft { get; set; }
    [ScoreColumn]
    public partial int PointsRound { get; set; }
    [ScoreColumn]
    public partial int PointsGame { get; set; }
}