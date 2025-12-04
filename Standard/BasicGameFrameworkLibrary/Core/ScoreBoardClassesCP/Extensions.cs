namespace BasicGameFrameworkLibrary.Core.ScoreBoardClassesCP;
public static class ScoreExtensions
{
    extension (BasicList<ScoreColumnModel> scores)
    {
        public BasicList<ScoreColumnModel> AddColumn(string header, bool isHorizontal, string normalPath, string visiblePath = "", EnumScoreSpecialCategory category = EnumScoreSpecialCategory.None)
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
}