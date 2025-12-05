namespace BladesOfSteel.Core.Logic;
public static class Extensions
{
    extension(IDeckDict<RegularSimpleCard> thisList)
    {
        public BasicList<BasicList<RegularSimpleCard>> PossibleCombinations(EnumRegularColorList whatColor)
        {
            return thisList.PossibleCombinations(whatColor, 3);
        }
        public BasicList<BasicList<RegularSimpleCard>> PossibleCombinations(EnumRegularColorList whatColor, int maxs)
        {
            if (maxs < 3 && whatColor == EnumRegularColorList.Red)
            {
                throw new CustomBasicException("Attack must allow 3 cards for combinations");
            }
            int mins;
            if (whatColor == EnumRegularColorList.Red)
            {
                mins = 2;
            }
            else
            {
                mins = 1;
            }
            int x;
            BasicList<int> firstList = new();
            var loopTo = maxs;
            for (x = mins; x <= loopTo; x++)
            {
                firstList.Add(x);
            }
            var newList = thisList.Where(items => items.Color == whatColor).ToBasicList();
            BasicList<BasicList<RegularSimpleCard>> fullList = new();
            firstList.ForEach(y =>
            {
                var thisCombo = newList.GetCombinations(y);
                fullList.AddRange(thisCombo);
            });
            return fullList;
        }
    }
    
}