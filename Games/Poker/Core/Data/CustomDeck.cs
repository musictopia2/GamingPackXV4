namespace Poker.Core.Data;
public class CustomDeck : IRegularDeckInfo
{
    int IRegularDeckInfo.HowManyDecks => 1; //most games use one deck.  if there is more than one deck, put here.
    bool IRegularDeckInfo.UseJokers => false;
    int IRegularDeckInfo.GetExtraJokers => 0;
    int IRegularDeckInfo.LowestNumber => 1;
    int IRegularDeckInfo.HighestNumber => 14;
    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new();
    BasicList<EnumSuitList> IRegularDeckInfo.SuitList => EnumSuitList.CompleteList;
    int IDeckCount.GetDeckCount()
    {
        return 52;
    }
}