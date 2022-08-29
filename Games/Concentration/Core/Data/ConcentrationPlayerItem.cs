namespace Concentration.Core.Data;
[UseScoreboard]
public partial class ConcentrationPlayerItem : PlayerSingleHand<RegularSimpleCard>
{
    [ScoreColumn]
    public int Pairs { get; set; }
}