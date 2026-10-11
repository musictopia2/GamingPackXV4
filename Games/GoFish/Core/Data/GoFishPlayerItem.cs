namespace GoFish.Core.Data;
[UseScoreboard]
public partial class GoFishPlayerItem : PlayerSingleHand<RegularSimpleCard>
{
    [ScoreColumn]
    public partial int Pairs { get; set; }
}