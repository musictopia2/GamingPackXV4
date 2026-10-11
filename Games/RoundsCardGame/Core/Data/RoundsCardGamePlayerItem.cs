namespace RoundsCardGame.Core.Data;
[UseScoreboard]
public partial class RoundsCardGamePlayerItem : PlayerTrick<EnumSuitList, RoundsCardGameCardInformation>
{
    [ScoreColumn]
    public partial int RoundsWon { get; set; }
    [ScoreColumn]
    public partial int CurrentPoints { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}