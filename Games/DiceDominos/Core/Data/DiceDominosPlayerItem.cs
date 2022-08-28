namespace DiceDominos.Core.Data;
[UseScoreboard]
public partial class DiceDominosPlayerItem : SimplePlayer
{
    [ScoreColumn]
    public int DominosWon { get; set; }
}