namespace SkuckCardGame.Core.Data;
[UseScoreboard]
public partial class SkuckCardGamePlayerItem : PlayerTrick<EnumSuitList, SkuckCardGameCardInformation>
{
    [ScoreColumn]
    public partial int StrengthHand { get; set; }
    [ScoreColumn]
    public partial string TieBreaker { get; set; } = "";
    [ScoreColumn]
    public partial int BidAmount { get; set; }
    [ScoreColumn]
    public partial bool BidVisible { get; set; }
    [ScoreColumn]
    public partial int PerfectRounds { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    [JsonIgnore]
    public PlayerBoardObservable<SkuckCardGameCardInformation>? TempHand { get; set; }

    public DeckRegularDict<SkuckCardGameCardInformation> SavedTemp { get; set; } = [];
}