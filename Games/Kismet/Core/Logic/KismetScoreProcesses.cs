﻿namespace Kismet.Core.Logic;
[SingletonGame]
public class KismetScoreProcesses : IYahtzeeStyle
{
    private readonly YahtzeeGameContainer<KismetDice> _gameContainer;
    private readonly ScoreContainer _scoreContainer;
    private readonly YahtzeeVMData<KismetDice> _model;
    BasicList<string> IYahtzeeStyle.GetBottomText => new()
    {
        "2 Pair Same Color",
        "3 Of A Kind",
        "Straight (30)",
        "Flush All Same Color (35)",
        "Full House",
        "Full House Same Color",
        "4 Of A Kind",
        "Yarborough",
        "Kismet (5 Of A Kind)"
    };
    bool IYahtzeeStyle.HasExceptionFor5Kind => true;
    int IYahtzeeStyle.BottomDescriptionWidth => 700;
    int IYahtzeeStyle.BonusAmount(int topScore)
    {
        if (topScore < 63)
        {
            return 0;
        }
        if (topScore <= 70)
        {
            return 35;
        }
        if (topScore <= 77)
        {
            return 55;
        }
        return 75;
    }
    void IYahtzeeStyle.Extra5OfAKind()
    {
        var firstList = _gameContainer.PlayerList!.ToBasicList();
        firstList.RemoveSpecificItem(_gameContainer.SingleInfo!);
        firstList.ForEach(Items => Items.MissNextTurn = true);
    }
    public KismetScoreProcesses(YahtzeeGameContainer<KismetDice> gameContainer,
        ScoreContainer scoreContainer,
        YahtzeeVMData<KismetDice> model
        )
    {
        _gameContainer = gameContainer;
        _scoreContainer = scoreContainer;
        _model = model;
    }
    BasicList<DiceInformation> IYahtzeeStyle.GetDiceList()
    {
        var firstList = _model.Cup!.DiceList.ToBasicList();
        BasicList<DiceInformation> output = new();
        firstList.ForEach(items =>
        {
            DiceInformation thisDice = new();
            thisDice.Value = items.Value;
            thisDice.Color = items.DotColor;
            output.Add(thisDice);
        });
        return output;
    }
    private bool HasFlush()
    {
        int count = _scoreContainer.DiceList.MaximumDuplicates(x => x.Color);
        return count == 5;
    }
    private bool HasTwoPairSameColor()
    {
        if (_scoreContainer.HasFullHouse() == true && HasFlush() == true)
        {
            return true;
        }
        if (_scoreContainer.HasKind(5))
        {
            return true;
        }
        var tempList = _scoreContainer.DiceList.GroupOrderDescending(x => x.Color);
        if (tempList.Count() > 2)
        {
            return false;
        }
        if (tempList.First().Count() < 4)
        {
            return false; //i did that in the old.  if that is not correct, rethink.
        }
        var first = _scoreContainer.DiceList.Where(x => x.Color == tempList.First().Key)
            .GroupOrderDescending(x => x.Value).ToBasicList();
        if (first.First().Count() == 4)
        {
            return true;
        }
        if (first.First().Count() != 2)
        {
            return false;
        }
        if (first[1].Count() != 2)
        {
            return false;
        }
        return true;
    }
    void IYahtzeeStyle.PopulateBottomScores()
    {
        if (HasTwoPairSameColor() == true && _scoreContainer.RowList[9].HasFilledIn() == false)
        {
            _scoreContainer.RowList[9].Possible = _scoreContainer.CalculateDiceTotal();
        }
        if (_scoreContainer.HasKind(3) == true && _scoreContainer.RowList[10].HasFilledIn() == false)
        {
            _scoreContainer.RowList[10].Possible = _scoreContainer.CalculateDiceTotal();
        }
        if (_scoreContainer.HasStraight(false) == true && _scoreContainer.RowList[11].HasFilledIn() == false)
        {
            _scoreContainer.RowList[11].Possible = 30;//
        }
        if (HasFlush() == true && _scoreContainer.RowList[12].HasFilledIn() == false)
        {
            _scoreContainer.RowList[12].Possible = 35;
        }
        if (_scoreContainer.HasFullHouse() == true && _scoreContainer.RowList[13].HasFilledIn() == false)
        {
            _scoreContainer.RowList[13].Possible = _scoreContainer.CalculateDiceTotal() + 15;
        }
        if (_scoreContainer.HasFullHouse() == true && HasFlush() == true && _scoreContainer.RowList[14].HasFilledIn() == false)
        {
            _scoreContainer.RowList[14].Possible = _scoreContainer.CalculateDiceTotal() + 20;
        }
        if (_scoreContainer.HasKind(4) == true && _scoreContainer.RowList[15].HasFilledIn() == false)
        {
            _scoreContainer.RowList[15].Possible = _scoreContainer.CalculateDiceTotal() + 25;
        }
        if (_scoreContainer.RowList[16].HasFilledIn() == false)
        {
            _scoreContainer.RowList[16].Possible = _scoreContainer.CalculateDiceTotal();
        }
        if (_scoreContainer.HasAllFive() == true && _scoreContainer.RowList[17].HasFilledIn() == false)
        {
            _scoreContainer.RowList[17].Possible = _scoreContainer.CalculateDiceTotal() + 50;
        }
    }
}