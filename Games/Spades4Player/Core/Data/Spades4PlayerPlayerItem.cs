namespace Spades4Player.Core.Data;
[UseScoreboard]
public partial class Spades4PlayerPlayerItem : PlayerTrick<EnumSuitList, Spades4PlayerCardInformation>
{
    [ScoreColumn]
    public partial int HowManyBids { get; set; }
    [ScoreColumn]
    public partial int Bags { get; set; }
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    public int Team { get; set; }
}