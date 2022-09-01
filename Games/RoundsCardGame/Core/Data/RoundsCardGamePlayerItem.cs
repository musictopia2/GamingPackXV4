namespace RoundsCardGame.Core.Data;
[UseScoreboard]
public partial class RoundsCardGamePlayerItem : PlayerTrick<EnumSuitList, RoundsCardGameCardInformation>
{
    [ScoreColumn]
    public int RoundsWon { get; set; }
    [ScoreColumn]
    public int CurrentPoints { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}