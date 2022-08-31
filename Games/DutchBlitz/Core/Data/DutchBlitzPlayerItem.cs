namespace DutchBlitz.Core.Data;
[UseScoreboard]
public partial class DutchBlitzPlayerItem : PlayerSingleHand<DutchBlitzCardInformation>
{
    [ScoreColumn]
    public int StockLeft { get; set; }
    [ScoreColumn]
    public int PointsRound { get; set; }
    [ScoreColumn]
    public int PointsGame { get; set; }
}