namespace PickelCardGame.Core.Cards;
public class CustomDeck : IRegularDeckInfo
{
    int IRegularDeckInfo.HowManyDecks => 1;
    bool IRegularDeckInfo.UseJokers => true;
    int IRegularDeckInfo.GetExtraJokers => 0;
    int IRegularDeckInfo.LowestNumber => 2;
    int IRegularDeckInfo.HighestNumber => 14;
    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new();
    BasicList<EnumSuitList> IRegularDeckInfo.SuitList => EnumSuitList.CompleteList;
    int IDeckCount.GetDeckCount()
    {
        return 54; //because of 2 jokers.
    }
}