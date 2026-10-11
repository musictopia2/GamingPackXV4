namespace Flinch.Core.Data;
[UseScoreboard]
public partial class FlinchPlayerItem : PlayerSingleHand<FlinchCardInformation>
{
    public DeckRegularDict<FlinchCardInformation> StockList { get; set; } = [];
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
    public BasicList<BasicPileInfo<FlinchCardInformation>> DiscardList { get; set; } = [];
}