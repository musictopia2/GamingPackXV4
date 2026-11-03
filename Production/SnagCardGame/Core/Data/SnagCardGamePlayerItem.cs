namespace SnagCardGame.Core.Data;
[UseScoreboard]
public partial class SnagCardGamePlayerItem : PlayerTrick<EnumSuitList, SnagCardGameCardInformation>
{
    [ScoreColumn]
    public partial int CardsWon { get; set; }
    [ScoreColumn]
    public partial int CurrentPoints { get; set; }
    [ScoreColumn]
    public partial int TotalPoints { get; set; }
}