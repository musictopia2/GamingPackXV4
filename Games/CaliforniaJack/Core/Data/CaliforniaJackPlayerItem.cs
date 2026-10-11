namespace CaliforniaJack.Core.Data;
[UseScoreboard]
public partial class CaliforniaJackPlayerItem : PlayerTrick<EnumSuitList, CaliforniaJackCardInformation>
{
    [ScoreColumn]
    public partial int Points { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}