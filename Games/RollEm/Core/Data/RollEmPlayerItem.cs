namespace RollEm.Core.Data;
[UseScoreboard]
public partial class RollEmPlayerItem : SimplePlayer
{
    [ScoreColumn]
    public int ScoreRound { get; set; }
    [ScoreColumn]
    public int ScoreGame { get; set; }
}