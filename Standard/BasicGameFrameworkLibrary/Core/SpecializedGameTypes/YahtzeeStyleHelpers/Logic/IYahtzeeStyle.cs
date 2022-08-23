namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;

/// <summary>
/// this is all the unique things that has to be done depending on game.
/// </summary>
public interface IYahtzeeStyle
{
    int BonusAmount(int topScore);
    BasicList<string> GetBottomText { get; }
    //now whoever implements is responsible for using proper information.
    void PopulateBottomScores();
    BasicList<DiceInformation> GetDiceList(); //i think
    void Extra5OfAKind(); //this is the processes for an extra 5 of a kind.
                          //will attempt to force somebody to get what is needed themselves.
    int BottomDescriptionWidth { get; } //the game has to decide what it will be where text does not get cut off.
    bool HasExceptionFor5Kind { get; }//kismet would be true.  others would be false.
}