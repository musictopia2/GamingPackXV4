namespace Concentration.Core.Data;
[UseScoreboard]
public partial class ConcentrationPlayerItem : PlayerSingleHand<RegularSimpleCard>
{
    [ScoreColumn]
    public partial int Pairs { get; set; }
}