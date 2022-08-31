namespace Skipbo.Core.Data;
[UseScoreboard]
public partial class SkipboPlayerItem : PlayerSingleHand<SkipboCardInformation>
{
    public BasicList<BasicPileInfo<SkipboCardInformation>> DiscardList { get; set; } = new();
    public DeckRegularDict<SkipboCardInformation> StockList { get; set; } = new DeckRegularDict<SkipboCardInformation>();
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
}