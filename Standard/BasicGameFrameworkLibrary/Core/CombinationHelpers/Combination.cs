namespace BasicGameFrameworkLibrary.Core.CombinationHelpers;
internal class Combination : IComparer<BasicList<object>>
{
    public static BasicList<BasicList<int>> CheckScores(int howManyExist, int howManyNeeded)
    {
        if (howManyNeeded > howManyExist)
        {
            return new();
        }
        object[] mySet;
        BasicList<BasicList<int>> arr_Return = new();
        mySet = new object[howManyExist - 1 + 1];
        var loopTo = howManyExist - 1;
        for (int i = 0; i <= loopTo; i++)
        {
            mySet[i] = i + 1;
        }
        BasicList<BasicList<object>> mySubsets = GetSubsets(mySet, howManyNeeded);
        Sort(mySubsets);
        foreach (BasicList<object> i in mySubsets)
        {
            BasicList<int> arr_Sample = new();
            foreach (int j in i.Select(v => (int)v))
            {
                arr_Sample.Add(j);
            }
            arr_Return.Add(arr_Sample);
        }

        return arr_Return;
    }
    /// <summary>
    /// Gets a nested(2D) list of unique combinations.
    /// </summary>
    /// <param name="items">A list(set) of objects from which
    /// the unique combinations of subsets should be returned.</param>
    /// <param name="k">The subset size.</param>
    public static BasicList<BasicList<object>> GetSubsets(object[] items, int k)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items), "The parameter can’t be Null.");
        }
        if (items.Length == 0)
        {
            throw new ArgumentException("The list must contain at least one object.", nameof(items));
        }

        int i = k;
        int n = items.Length;
        if (i - 1 < 0 || k > n)
        {
            throw new ArgumentOutOfRangeException(nameof(k), k, "The value must be in the range {1 to " + Convert.ToString(n) + "}.");
        }
        BasicList<BasicList<object>> finalList = new();
        i -= 1;
        int[] indexs = new int[k - 1 + 1];
        BasicList<object> firstSubList = new();
        var loopTo = k - 1;
        for (int j = 0; j <= loopTo; j++)
        {
            indexs[j] = j;
            firstSubList.Add(items[j]);
        }
        finalList.Add(firstSubList);
        while (indexs[0] != n - k && finalList.Count < 2147483647)
        {
            if (indexs[i] < i + (n - k))
            {
                indexs[i] += 1;
                BasicList<object> subList = new();
                foreach (int j in indexs)
                {
                    subList.Add(items[j]);
                }
                finalList.Add(subList);
            }
            else
            {
                do
                {
                    i -= 1;
                }
                while (indexs[i] == i + (n - k));
                indexs[i] += 1;
                var loopTo1 = k - 1;
                for (int j = i + 1; j <= loopTo1; j++)
                {
                    indexs[j] = indexs[j - 1] + 1;
                }
                BasicList<object> subList = new();
                foreach (int j in indexs)
                {
                    subList.Add(items[j]);
                }
                finalList.Add(subList);
                i = k - 1;
            }
        }

        return finalList;
    }
    /// <summary>
    /// Sorts the elements in a List(Of List(Of Object)).
    /// </summary>
    /// <param name="combLists">A list of combinations list to sort.</param>
    public static void Sort(BasicList<BasicList<object>> combLists)
    {
        if (combLists.Count == 1)
        {
            combLists[0].Sort();
        }
        else if (combLists.Count > 1)
        {
            try
            {
                combLists.Sort(new Combination());
            }
            catch (Exception)
            {
                combLists[0].Sort();
            }
        }
    }
    /// <summary>
    /// Compares two specified List(Of Object).
    /// </summary>
    /// <param name="x">The first List(Of Object).</param>
    /// <param name="y">The second List(Of Object).</param>
    public virtual int Compare(BasicList<object>? x, BasicList<object>? y)
    {
        x!.Sort();
        y!.Sort();
        int t;
        string value = x[0].ToString()!;
        if (double.TryParse(value, out _) == true) //hopefully that works.
        {
            var loopTo = x.Count - 1;
            for (int i = 0; i <= loopTo; i++)
            {
                value = y[i].ToString()!;
                t = double.Parse(Convert.ToString(i)).CompareTo(double.Parse(value)); //hopefully this works.
                if (t != 0)
                {
                    return t;
                }
            }
            return 0;
        }
        if (x[0].GetType() == typeof(string))
        {
            var loopTo1 = x.Count - 1;
            for (int i = 0; i <= loopTo1; i++)
            {
                t = Convert.ToString(x[i])!.CompareTo(y[i]);
                if (t != 0)
                {
                    return t;
                }
            }
            return 0;
        }
        if (x[0].GetType() == typeof(DateTime))
        {
            var loopTo2 = x.Count - 1;
            for (int i = 0; i <= loopTo2; i++)
            {
                t = Convert.ToDateTime(x[i]).CompareTo(y[i]); //hopefully this works.
                if (t != 0)
                {
                    return t;
                }
            }
            return 0;
        }
        return 0;
    }
}