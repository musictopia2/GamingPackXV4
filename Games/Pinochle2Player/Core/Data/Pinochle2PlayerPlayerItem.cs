namespace Pinochle2Player.Core.Data;
[UseScoreboard]
public partial class Pinochle2PlayerPlayerItem : PlayerTrick<EnumSuitList, Pinochle2PlayerCardInformation>
{
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
}