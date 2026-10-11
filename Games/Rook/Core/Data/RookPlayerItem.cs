namespace Rook.Core.Data;
[UseScoreboard]
public partial class RookPlayerItem : PlayerTrick<EnumColorTypes, RookCardInformation>
{
    public bool Pass { get; set; }
    [ScoreColumn]
    public partial int BidAmount { get; set; }
    [ScoreColumn]
    public partial int CurrentScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    public int Team { get; set; } //this is only useful for 4 player.  always somehow works for 3 player.
}