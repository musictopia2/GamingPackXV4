namespace RollEm.Core.Logic;
public static class ComputerAI
{
    private struct ComboInfo
    {
        public int Number1;
        public int Number2;
    }
    public static BasicList<int> NumberList(GameBoardProcesses gameBoard1)
    {
        BasicList<int> output = new();
        var thisList = gameBoard1.GetNumberList();
        var newList = ComboList(thisList, gameBoard1);
        if (newList.Count == 0)
        {
            return new();
        }
        var thisCombo = newList.GetRandomItem();
        if (thisCombo.Number1 == 0)
        {
            throw new CustomBasicException("The first number at least has to be filled out");
        }
        if (gameBoard1.GetDiceTotal != (thisCombo.Number1 + thisCombo.Number2))
        {
            throw new CustomBasicException("This was not even correct");
        }
        output.Add(thisCombo.Number1);
        if (thisCombo.Number2 > 0)
        {
            output.Add(thisCombo.Number2);
        }
        return output;
    }
    private static BasicList<ComboInfo> ComboList(BasicList<int> thisList, GameBoardProcesses gameBoard1)
    {
        BasicList<ComboInfo> output;
        int totals = gameBoard1.GetDiceTotal;
        output = thisList.Where(xx => xx == totals).Select(xx => new ComboInfo { Number1 = xx, Number2 = 0 }).ToBasicList();
        int x;
        var loopTo = thisList.Count - 1;
        for (x = 0; x <= loopTo; x++)
        {
            var loopTo1 = thisList.Count - 1;
            for (var y = x + 1; y <= loopTo1; y++)
            {
                if ((thisList[x] + thisList[y]) == totals)
                {
                    ComboInfo combo = new ();
                    combo.Number1 = thisList[x];
                    combo.Number2 = thisList[y];
                    output.Add(combo);
                }
            }
        }
        return output;
    }
}