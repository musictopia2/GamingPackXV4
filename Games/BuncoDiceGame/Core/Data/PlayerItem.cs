namespace BuncoDiceGame.Core.Data;
[UseScoreboard]
public partial class PlayerItem : SimplePlayer
{
    [ScoreColumn]
    public int Team { get; set; }
    [ScoreColumn]
    public int Buncos { get; set; }
    [ScoreColumn]
    public int Wins { get; set; }
    [ScoreColumn]
    public int Losses { get; set; }
    public int PreviousMate { get; set; }
    [ScoreColumn]
    public int Points { get; set; }
    public bool WonPrevious { get; set; }
    [ScoreColumn]
    public int Table { get; set; }
    public bool WinDetermined { get; set; }
    public bool Acceptable { get; set; }
    public int PlayerNum { get; set; }
    public int TempTable { get; set; }
}