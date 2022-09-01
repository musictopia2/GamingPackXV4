namespace CaliforniaJack.Core.Data;
[UseScoreboard]
public partial class CaliforniaJackPlayerItem : PlayerTrick<EnumSuitList, CaliforniaJackCardInformation>
{
    [ScoreColumn]
    public int Points { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}