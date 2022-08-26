
namespace DominoBonesMultiplayerGames.Core.Data;
[UseScoreboard]
public partial class DominoBonesMultiplayerGamesPlayerItem : PlayerSingleHand<SimpleDominoInfo>
{
    [ScoreColumn] //does not hurt making as scorecolumn
    public int TotalScore { get; set; }
}