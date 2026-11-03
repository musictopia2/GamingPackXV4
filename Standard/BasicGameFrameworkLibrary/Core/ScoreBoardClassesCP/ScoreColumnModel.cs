namespace BasicGameFrameworkLibrary.Core.ScoreBoardClassesCP;
public class ScoreColumnModel
{
    public string MainPath { get; set; } = "";
    public string Header { get; set; } = "";
    public string VisiblePath { get; set; } = "";
    public bool IsHorizontal { get; set; } = true;
    public int NumberOfDigits { get; set; } = 0;
    public EnumScoreSpecialCategory SpecialCategory { get; set; } = EnumScoreSpecialCategory.None;
}