namespace Skipbo.Core.Data;
[UseScoreboard]
public partial class SkipboPlayerItem : PlayerSingleHand<SkipboCardInformation>
{
    public BasicList<BasicPileInfo<SkipboCardInformation>> DiscardList { get; set; } = [];
    public DeckRegularDict<SkipboCardInformation> StockList { get; set; } = [];
    [ScoreColumn]
    public partial string InStock { get; set; } = "";
    [ScoreColumn]
    public partial int StockLeft { get; set; }
    [ScoreColumn]
    public partial string Discard1 { get; set; } = "";
    [ScoreColumn]
    public partial string Discard2 { get; set; } = "";
    [ScoreColumn]
    public partial string Discard3 { get; set; } = "";
    [ScoreColumn]
    public partial string Discard4 { get; set; } = "";
    [ScoreColumn]
    public partial string Discard5 { get; set; } = "";
}