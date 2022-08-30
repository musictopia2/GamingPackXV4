namespace Chinazo.Core.Logic;
public class CustomDeck : IRegularDeckInfo
{
    int IRegularDeckInfo.HowManyDecks => 2;
    bool IRegularDeckInfo.UseJokers => true;
    int IRegularDeckInfo.GetExtraJokers => 2;
    int IRegularDeckInfo.LowestNumber => 2;
    int IRegularDeckInfo.HighestNumber => 14;
    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new();
    BasicList<EnumSuitList> IRegularDeckInfo.SuitList => EnumSuitList.CompleteList;
    int IDeckCount.GetDeckCount()
    {
        return 110;  //decided 2 extra jokers.
    }
}