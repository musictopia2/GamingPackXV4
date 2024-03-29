﻿namespace ThinkTwice.Core.Logic;
[SingletonGame]
[AutoReset]
public class ScoreLogic
{
    private readonly ThinkTwiceVMData _model;
    private readonly ThinkTwiceGameContainer _gameContainer;
    public ScoreLogic(ThinkTwiceVMData model, ThinkTwiceGameContainer gameContainer)
    {
        _model = model;
        _gameContainer = gameContainer;
    }
    public int CalculateScore()
    {
        int index = _model.ItemSelected;
        if (index == -1)
        {
            throw new CustomBasicException("Cannot calculate the score because nothing was selected");
        }
        if (_gameContainer.SaveRoot!.CategoryRolled == -1)
        {
            throw new CustomBasicException("Cannot calculate the score because the category dice has not been rolled");
        }
        if (_gameContainer.SaveRoot.WhichMulti == 0)
        {
            return 0;
        }
        bool sameCategory;
        sameCategory = WasSameCategory();
        int newMulti = MultiToUse(sameCategory);
        BasicList<SimpleDice> tempList;
        tempList = _model.Cup!.DiceList.ToBasicList();
        BasicList<int> mainList = new();
        foreach (var thisDice in tempList)
        {
            mainList.Add(thisDice.Value);
        }
        int scoress;
        scoress = FirstScore(mainList, out bool Additionals);
        if (Additionals == true)
        {
            scoress *= 2;
        }
        return scoress * newMulti;
    }
    private int FirstScore(BasicList<int> thisCol, out bool receiveMults)
    {
        int index;
        index = _model.ItemSelected; ;
        if (index == 5)
        {
            int maxs;
            maxs = MaxKinds(thisCol, out receiveMults);
            if (maxs == 2)
            {
                return 10;
            }
            if (maxs == 3)
            {
                return 20;
            }
            if (maxs == 4)
            {
                return 30;
            }
            if (maxs == 5)
            {
                return 50;
            }
            if (maxs == 6)
            {
                return 100;
            }
            return 0;
        }
        if (index == 0)
        {
            return ScoreDifferent(thisCol, out receiveMults);
        }
        int x;
        BasicList<int> newCol = new();
        if (index == 3)
        {
            for (x = 1; x <= 3; x++)
            {
                newCol.Add(x);
            }
            int tempScore;
            tempScore = ScoreCombo(thisCol, newCol, out receiveMults);
            return tempScore + 10;
        }
        if (index == 1)
        {
            newCol.Add(2);
            newCol.Add(4);
            newCol.Add(6);
        }
        else if (index == 2)
        {
            for (x = 4; x <= 6; x++)
            {
                newCol.Add(x);
            }
        }
        else if (index == 4)
        {
            newCol.Add(1);
            newCol.Add(3);
            newCol.Add(5);
        }
        else
        {
            throw new CustomBasicException("Can't find the combo for index " + index);
        }
        return ScoreCombo(thisCol, newCol, out receiveMults);
    }
    private static int ScoreCombo(BasicList<int> mainCol, BasicList<int> comboList, out bool receiveMults)
    {
        int sums = mainCol.Where(items => comboList.Contains(items)).Sum();
        int counts = mainCol.Where(items => comboList.Contains(items)).Count();
        if (counts == 6)
        {
            receiveMults = true;
        }
        else
        {
            receiveMults = false;
        }
        return sums;
    }
    private static int ScoreDifferent(BasicList<int> thisCol, out bool receiveMults)
    {
        var temps = thisCol.GroupBy(Items => Items).ToBasicList();
        if (temps.Count == 6)
        {
            receiveMults = true;
            return thisCol.Sum(Items => Items);
        }
        else
        {
            receiveMults = false;
            return temps.Sum(Items => Items.Key);
        }
    }
    private static int MaxKinds(BasicList<int> thisCol, out bool receiveMults)
    {
        int counts = thisCol.MaximumDuplicates();
        if (counts == 6)
        {
            receiveMults = true;
        }
        else
        {
            receiveMults = false;
        }
        return counts;
    }
    private int MultiToUse(bool sameCategory)
    {
        int firstMulti;
        if (_gameContainer.SaveRoot!.WhichMulti == -1)
        {
            firstMulti = 1;
        }
        else
        {
            firstMulti = _gameContainer.SaveRoot.WhichMulti;
        }
        if (sameCategory == false)
        {
            return firstMulti;
        }
        return firstMulti * 2;
    }
    private bool WasSameCategory()
    {
        int index;
        index = _model.ItemSelected;
        if (index + 1 == _gameContainer.SaveRoot!.CategoryRolled)
        {
            return true;
        }
        return false;
    }
}