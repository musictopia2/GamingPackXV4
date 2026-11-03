namespace Phase10.Core.Data;
[UseScoreboard]
public partial class Phase10PlayerItem : PlayerRummyHand<Phase10CardInformation>
{
    [ScoreColumn]
    public partial int Phase { get; set; } = 1;
    [ScoreColumn]
    public partial bool Completed { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
}