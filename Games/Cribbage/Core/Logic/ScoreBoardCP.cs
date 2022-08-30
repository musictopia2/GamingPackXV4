namespace Cribbage.Core.Logic;
public class ScoreBoardCP
{
    private readonly BasicList<ScoreInfo> _tempList = new();
    public BasicList<ScoreInfo> ScoreList = new();
    public int TotalScore => _tempList.Sum(items => items.Score);
    public void AddScore(string description, int score)
    {
        ScoreInfo thisScore = new();
        thisScore.Description = description;
        thisScore.Score = score;
        _tempList.Add(thisScore);
    }
    public void ShowScores()
    {
        ScoreList.ReplaceRange(_tempList);
    }
    public void ResetScores()
    {
        if (_tempList.Count == 0)
        {
            return; //already done.
        }
        _tempList.Clear();
        ScoreList.Clear();
    }
}