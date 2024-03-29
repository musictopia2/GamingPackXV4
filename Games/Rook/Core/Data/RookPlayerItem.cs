namespace Rook.Core.Data;
[UseScoreboard]
public partial class RookPlayerItem : PlayerTrick<EnumColorTypes, RookCardInformation>
{

    //try to not even do the IsDummy.  especially since we may have 4 players eventually.

    //public bool IsDummy { get; set; }
    public bool Pass { get; set; }
    [ScoreColumn]
    public int BidAmount { get; set; }
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
    public int Team { get; set; } //this is only useful for 4 player.  always somehow works for 3 player.
}