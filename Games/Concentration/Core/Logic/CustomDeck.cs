namespace Concentration.Core.Logic;
public class CustomDeck : IRegularDeckInfo
{
    public int HowManyDecks => 1;
    public bool UseJokers => false;
    public int GetExtraJokers => 0;
    public int LowestNumber => 1;
    public int HighestNumber => 13;
    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList
    {
        get
        {
            BasicList<ExcludeRCard> output = new();
            output.AppendExclude(EnumSuitList.Clubs, 1)
                .AppendExclude(EnumSuitList.Spades, 1);
            return output;
        }
    }
    public BasicList<EnumSuitList> SuitList => EnumSuitList.CompleteList;
    public int GetDeckCount()
    {
        return 50;
    }
}