namespace BasicGameFrameworkLibrary.Core.ScoreBoardClassesCP;
public static class ScoreExtensions
{
    public static BasicList<ScoreColumnModel> AddColumn(this BasicList<ScoreColumnModel> scores, string header, bool isHorizontal, string normalPath, string visiblePath = "", EnumScoreSpecialCategory category = EnumScoreSpecialCategory.None)
    {
        if (scores.Count == 0 && header != "Nick Name")
        {
            scores = scores.AddColumn("Nick Name", true, nameof(IPlayerItem.NickName));
        }
        ScoreColumnModel info = new()
        {
            Header = header,
            MainPath = normalPath,
            IsHorizontal = isHorizontal,
            SpecialCategory = category,
            VisiblePath = visiblePath,
        };
        scores.Add(info);
        return scores;
    }
}