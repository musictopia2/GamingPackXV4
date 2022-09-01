namespace Pinochle2Player.Core.Cards;
public class CustomDeck : IRegularDeckInfo
{
    int IRegularDeckInfo.HowManyDecks => 2;
    bool IRegularDeckInfo.UseJokers => false;
    int IRegularDeckInfo.GetExtraJokers => 0;
    int IRegularDeckInfo.LowestNumber => 9;
    int IRegularDeckInfo.HighestNumber => 14;
    public BasicList<ExcludeRCard> ExcludeList => new();
    public BasicList<EnumSuitList> SuitList => EnumSuitList.CompleteList;
    int IDeckCount.GetDeckCount()
    {
        return 48;
    }
}