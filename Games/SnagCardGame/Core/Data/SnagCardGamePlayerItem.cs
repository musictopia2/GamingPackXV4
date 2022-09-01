namespace SnagCardGame.Core.Data;
[UseScoreboard]
public partial class SnagCardGamePlayerItem : PlayerTrick<EnumSuitList, SnagCardGameCardInformation>
{
    [ScoreColumn]
    public int CardsWon { get; set; }
    [ScoreColumn]
    public int CurrentPoints { get; set; }
    [ScoreColumn]
    public int TotalPoints { get; set; }
}