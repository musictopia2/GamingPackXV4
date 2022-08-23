namespace BasicGameFrameworkLibrary.Core.CombinationHelpers;
public static class Extensions
{
    public static BasicList<BasicList<T>> GetCombinations<T>(this BasicList<T> thisList, int howMany)
    {
        BasicList<BasicList<int>> tempList;
        tempList = Combination.CheckScores(thisList.Count, howMany);
        BasicList<BasicList<T>> newList = new();
        tempList.ForEach(firstItem =>
        {
            var fins = new BasicList<T>();
            firstItem.ForEach(secondItem =>
            {
                fins.Add(thisList[secondItem - 1]);
            });
            newList.Add(fins);
        });
        return newList;
    }
    public static BasicList<BasicList<T>> GetAllPossibleCombinations<T>(this BasicList<T> thisList)
    {
        BasicList<BasicList<T>> finList = new();
        var loopTo = thisList.Count;
        int x;
        for (x = 1; x <= loopTo; x++)
        {
            BasicList<BasicList<T>> tempList = thisList.GetCombinations(x);
            finList.AddRange(tempList);
        }
        return finList;
    }
}