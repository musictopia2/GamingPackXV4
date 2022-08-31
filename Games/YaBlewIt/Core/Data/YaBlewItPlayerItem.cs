namespace YaBlewIt.Core.Data;
[UseScoreboard]
public partial class YaBlewItPlayerItem : PlayerSingleHand<YaBlewItCardInformation>
{
    [ScoreColumn]
    public EnumColors CursedGem { get; set; }
    [ScoreColumn]
    public int TotalScore => MainHandList.TotalPoints(this);
}