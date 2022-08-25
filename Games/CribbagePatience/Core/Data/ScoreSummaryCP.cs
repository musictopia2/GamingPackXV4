namespace CribbagePatience.Core.Data;
public class ScoreSummaryCP
{
    public BasicList<int> ScoreList
    {
        get => _mainGame._saveRoot.ScoreList;
        set => _mainGame._saveRoot.ScoreList = value;
    }
    private readonly CribbagePatienceMainGameClass _mainGame;
    public ScoreSummaryCP()
    {
        _mainGame = aa.Resolver!.Resolve<CribbagePatienceMainGameClass>();
    }
    public int TotalScore
    {
        get
        {
            return ScoreList.Sum();
        }
    }
    public void AddScore(int Score)
    {
        ScoreList.Add(Score);
    }
    public void NewGame()
    {
        ScoreList.Clear();
    }
}