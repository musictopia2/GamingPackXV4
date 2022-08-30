namespace CoveredUp.Core.Data;
[UseScoreboard]
public partial class CoveredUpPlayerItem : PlayerSingleHand<RegularSimpleCard>
{
    [ScoreColumn]
    public int PointsSoFar { get; set; }
    [ScoreColumn]
    public int PreviousScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
    [JsonIgnore]
    public PlayerBoardCP? PlayerBoard { get; set; }
    public void LoadPlayerBoard(CoveredUpGameContainer gameContainer, CoveredUpVMData model)
    {
        PlayerBoard = new(gameContainer, model);
        PlayerBoard.LoadBoard(this);
    }
}