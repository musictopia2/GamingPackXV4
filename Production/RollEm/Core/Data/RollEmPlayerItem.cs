namespace RollEm.Core.Data;
[UseScoreboard]
public partial class RollEmPlayerItem : SimplePlayer
{
    [ScoreColumn]
    public partial int ScoreRound { get; set; }
    [ScoreColumn]
    public partial int ScoreGame { get; set; }
}