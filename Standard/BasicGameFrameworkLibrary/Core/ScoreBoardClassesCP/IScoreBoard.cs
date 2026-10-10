namespace BasicGameFrameworkLibrary.Core.ScoreBoardClassesCP;
public interface IScoreBoard
{
    event Action<string>? ScoreValueChanged;
    string TextToDisplay(
        ScoreColumnModel column,
        bool useAbbreviationForTrueFalse);
}