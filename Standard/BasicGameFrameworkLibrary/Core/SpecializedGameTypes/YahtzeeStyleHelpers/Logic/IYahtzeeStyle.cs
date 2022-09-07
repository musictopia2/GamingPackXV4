namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
/// <summary>
/// this is all the unique things that has to be done depending on game.
/// </summary>
public interface IYahtzeeStyle
{
    int BonusAmount(int topScore);
    BasicList<string> GetBottomText { get; }
    void PopulateBottomScores();
    BasicList<DiceInformation> GetDiceList();
    void Extra5OfAKind();
    int BottomDescriptionWidth { get; }
    bool HasExceptionFor5Kind { get; }
}