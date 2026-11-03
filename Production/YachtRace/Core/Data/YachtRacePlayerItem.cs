namespace YachtRace.Core.Data;
[UseScoreboard]
public partial class YachtRacePlayerItem : SimplePlayer
{
    [ScoreColumn]
    public partial float Time { get; set; }
}