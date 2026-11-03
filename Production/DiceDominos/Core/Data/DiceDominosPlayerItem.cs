namespace DiceDominos.Core.Data;
[UseScoreboard]
public partial class DiceDominosPlayerItem : SimplePlayer
{
    [ScoreColumn]
    public partial int DominosWon { get; set; }
}