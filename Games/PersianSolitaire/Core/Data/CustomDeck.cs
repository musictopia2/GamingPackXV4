namespace PersianSolitaire.Core.Data;
public class CustomDeck : IRegularDeckInfo
{
    int IRegularDeckInfo.HowManyDecks => 2;
    bool IRegularDeckInfo.UseJokers => false;
    int IRegularDeckInfo.GetExtraJokers => 0;
    int IRegularDeckInfo.LowestNumber => 7;
    int IRegularDeckInfo.HighestNumber => 14;
    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new();
    BasicList<EnumSuitList> IRegularDeckInfo.SuitList => EnumSuitList.CompleteList;
    int IDeckCount.GetDeckCount()
    {
        return 64;
    }
}