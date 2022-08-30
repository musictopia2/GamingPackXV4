namespace GolfCardGame.Core.Data;
public class GolfDeck : IRegularDeckInfo
{
    int IRegularDeckInfo.HowManyDecks => 1;

    bool IRegularDeckInfo.UseJokers => true;

    int IRegularDeckInfo.GetExtraJokers => 0;

    int IRegularDeckInfo.LowestNumber => 1;

    int IRegularDeckInfo.HighestNumber => 13;

    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new();

    BasicList<EnumSuitList> IRegularDeckInfo.SuitList => EnumSuitList.CompleteList;

    int IDeckCount.GetDeckCount()
    {
        return 54;
    }
}