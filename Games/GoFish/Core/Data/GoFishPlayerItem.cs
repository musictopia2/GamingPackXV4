namespace GoFish.Core.Data;
[UseScoreboard]
public partial class GoFishPlayerItem : PlayerSingleHand<RegularSimpleCard>
{
    [ScoreColumn]
    public int Pairs { get; set; }
}