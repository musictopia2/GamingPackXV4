namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;

public interface IScoreLogic
{
    void LoadBoard();
    void PopulatePossibleScores();
    void ClearRecent();
    void StartTurn();
    void MarkScore(RowInfo currentRow);
    int TotalScore { get; }
    BasicList<RowInfo> GetAvailableScores { get; }
}