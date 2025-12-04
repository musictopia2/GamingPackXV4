namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public static class Helpers
{
    extension (BasicList<ExcludeRCard> list)
    {
        public BasicList<ExcludeRCard> AppendExclude(EnumSuitList suit, int number)
        {
            list.Add(new ExcludeRCard(suit, number));
            return list;
        }
    }
   extension<R> (IEnumerable<R> list)
        where R: IRegularCard
    {
        public BasicList<EnumRegularCardValueList> LoadValuesFromCards()
        {
            return list.DistinctItems(xx => xx.Value);
        }
    }   
}