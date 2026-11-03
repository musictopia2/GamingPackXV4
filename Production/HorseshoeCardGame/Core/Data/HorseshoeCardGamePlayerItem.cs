namespace HorseshoeCardGame.Core.Data;
[UseScoreboard]
public partial class HorseshoeCardGamePlayerItem : PlayerTrick<EnumSuitList, HorseshoeCardGameCardInformation>
{
    [ScoreColumn]
    public partial int PreviousScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    [JsonIgnore]
    public PlayerBoardObservable<HorseshoeCardGameCardInformation>? TempHand { get; set; }
    public DeckRegularDict<HorseshoeCardGameCardInformation> SavedTemp { get; set; } = [];
}