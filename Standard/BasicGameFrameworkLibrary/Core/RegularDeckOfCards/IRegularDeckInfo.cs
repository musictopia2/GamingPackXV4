namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public interface IRegularDeckInfo : IDeckCount 
{
    int HowManyDecks { get; }
    bool UseJokers { get; }
    int GetExtraJokers { get; }
    int LowestNumber { get; }
    int HighestNumber { get; }
    BasicList<ExcludeRCard> ExcludeList { get; }
    BasicList<EnumSuitList> SuitList { get; }
}