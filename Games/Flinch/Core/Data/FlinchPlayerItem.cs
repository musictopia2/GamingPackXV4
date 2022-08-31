namespace Flinch.Core.Data;
[UseScoreboard]
public partial class FlinchPlayerItem : PlayerSingleHand<FlinchCardInformation>
{
    public DeckRegularDict<FlinchCardInformation> StockList { get; set; } = new DeckRegularDict<FlinchCardInformation>();
    [ScoreColumn]
    public string InStock { get; set; } = "";
    [ScoreColumn]
    public int StockLeft { get; set; }
    [ScoreColumn]
    public string Discard1 { get; set; } = "";
    [ScoreColumn]
    public string Discard2 { get; set; } = "";
    [ScoreColumn]
    public string Discard3 { get; set; } = "";
    [ScoreColumn]
    public string Discard4 { get; set; } = "";
    [ScoreColumn]
    public string Discard5 { get; set; } = "";
    public BasicList<BasicPileInfo<FlinchCardInformation>> DiscardList { get; set; } = new();
}