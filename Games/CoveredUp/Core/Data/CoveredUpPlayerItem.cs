namespace CoveredUp.Core.Data;
[UseScoreboard]
public partial class CoveredUpPlayerItem : PlayerSingleHand<RegularSimpleCard>
{
    [ScoreColumn]
    public partial int PointsSoFar { get; set; }
    [ScoreColumn]
    public partial int PreviousScore { get; set; }
    [ScoreColumn]
    public partial int TotalScore { get; set; }
    [JsonIgnore]
    public PlayerBoardCP? PlayerBoard { get; set; }
    public void LoadPlayerBoard(CoveredUpGameContainer gameContainer, CoveredUpVMData model)
    {
        PlayerBoard = new(gameContainer, model);
        PlayerBoard.LoadBoard(this);
    }
}