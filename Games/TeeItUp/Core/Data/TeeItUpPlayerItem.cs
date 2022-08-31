namespace TeeItUp.Core.Data;
[UseScoreboard]
public partial class TeeItUpPlayerItem : PlayerSingleHand<TeeItUpCardInformation>
{
    [JsonIgnore]
    public TeeItUpPlayerBoardCP? PlayerBoard { get; set; }
    public bool FinishedChoosing { get; set; }
    [ScoreColumn]
    public bool WentOut { get; set; }
    [ScoreColumn]
    public int PreviousScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
    public void LoadPlayerBoard(TeeItUpGameContainer gameContainer)
    {
        PlayerBoard = new (gameContainer);
        PlayerBoard.LoadBoard(this);
    }
}