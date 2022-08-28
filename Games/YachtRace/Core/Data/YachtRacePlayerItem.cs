namespace YachtRace.Core.Data;
[UseScoreboard]
public partial class YachtRacePlayerItem : SimplePlayer
{
    [ScoreColumn]
    public float Time { get; set; }
}