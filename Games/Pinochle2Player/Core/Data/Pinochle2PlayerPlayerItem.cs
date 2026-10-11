namespace Pinochle2Player.Core.Data;
[UseScoreboard]
public partial class Pinochle2PlayerPlayerItem : PlayerTrick<EnumSuitList, Pinochle2PlayerCardInformation>
{
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}