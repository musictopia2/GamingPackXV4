namespace SixtySix2Player.Core.Data;
[UseScoreboard]
public partial class SixtySix2PlayerPlayerItem : PlayerTrick<EnumSuitList, SixtySix2PlayerCardInformation>
{
    [ScoreColumn]
    public partial int ScoreRound { get; set; }
    [ScoreColumn]
    public partial int GamePointsRound { get; set; }
    [ScoreColumn]
    public partial int GamePointsGame { get; set; }
    public EnumMarriage FirstMarriage { get; set; }
    public BasicList<EnumMarriage> MarriageList { get; set; } = [];
}