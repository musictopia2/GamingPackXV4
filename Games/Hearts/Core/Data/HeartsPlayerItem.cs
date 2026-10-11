namespace Hearts.Core.Data;
[UseScoreboard]
public partial class HeartsPlayerItem : PlayerTrick<EnumSuitList, HeartsCardInformation>
{
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int PreviousScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    [ScoreColumn]
    public partial bool HadPoints { get; set; }
    public BasicList<int> CardsPassed { get; set; } = [];
}