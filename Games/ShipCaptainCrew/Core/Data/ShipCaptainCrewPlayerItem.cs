namespace ShipCaptainCrew.Core.Data;
[UseScoreboard]
public partial class ShipCaptainCrewPlayerItem : SimplePlayer
{
    [ScoreColumn]
    public partial bool WentOut { get; set; }
    [ScoreColumn]
    public partial int Score { get; set; }
    [ScoreColumn]
    public partial int Wins { get; set; }
    [ScoreColumn]
    public partial bool TookTurn { get; set; }
}