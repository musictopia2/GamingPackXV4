namespace OldMaid.Core.Data;
public class OldMaidDeck : IRegularDeckInfo
{
    int IRegularDeckInfo.HowManyDecks => 1;

    bool IRegularDeckInfo.UseJokers => false;

    int IRegularDeckInfo.GetExtraJokers => 0;

    int IRegularDeckInfo.LowestNumber => 2;

    int IRegularDeckInfo.HighestNumber => 14;

    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList
    {
        get
        {
            BasicList<ExcludeRCard> output = new();
            output.AppendExclude(EnumSuitList.Clubs, 12)
                .AppendExclude(EnumSuitList.Diamonds, 12)
                .AppendExclude(EnumSuitList.Hearts, 12);
            return output;
        }
    }
    BasicList<EnumSuitList> IRegularDeckInfo.SuitList => EnumSuitList.CompleteList;
    int IDeckCount.GetDeckCount()
    {
        return 49;
    }
}