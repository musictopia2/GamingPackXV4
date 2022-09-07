namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public static class Helpers
{
    public static BasicList<ExcludeRCard> AppendExclude(this BasicList<ExcludeRCard> thisList,
        EnumSuitList suit, int number)
    {
        thisList.Add(new ExcludeRCard(suit, number));
        return thisList;
    }
    public static BasicList<EnumRegularCardValueList> LoadValuesFromCards<R>(this IDeckDict<R> thisDict) where R : IRegularCard
    {
        return thisDict.DistinctItems(xx => xx.Value);
    }
}