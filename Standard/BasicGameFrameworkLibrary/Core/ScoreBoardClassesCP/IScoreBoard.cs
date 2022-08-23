namespace BasicGameFrameworkLibrary.Core.ScoreBoardClassesCP;

public interface IScoreBoard
{
    string TextToDisplay(ScoreColumnModel column, bool useAbbreviationForTrueFalse);
}