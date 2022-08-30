﻿namespace MonasteryCardGame.Core.Logic;
public class RummySet : SetInfo<EnumSuitList, EnumRegularColorList, MonasteryCardInfo, SavedSet>
{
    private EnumMonasterySets _setType;
    private readonly MonasteryCardGameGameContainer _gameContainer;
    public RummySet(MonasteryCardGameGameContainer gameContainer) : base(gameContainer.Command)
    {
        CanExpandRuns = true;
        _gameContainer = gameContainer;
    }
    public override void LoadSet(SavedSet payLoad)
    {
        _setType = payLoad.WhatType;
        HandList.ReplaceRange(payLoad.CardList);
    }
    public override SavedSet SavedSet()
    {
        SavedSet output = new();
        output.WhatType = _setType;
        output.CardList = HandList.ToRegularDeckDict();
        return output;
    }
    protected override bool IsRun()
    {
        return _setType == EnumMonasterySets.DoubleRun || _setType == EnumMonasterySets.RegularRuns || _setType == EnumMonasterySets.RunColors || _setType == EnumMonasterySets.SuitRuns;
    }
    protected override bool CanClickMainBoard()
    {
        return true; //maybe it can now even with desktop because of timings.  learned from rummy 500.
    }
    public void CreateSet(IDeckDict<MonasteryCardInfo> thisCol, EnumMonasterySets whatSet)
    {
        _setType = whatSet;
        DeckRegularDict<MonasteryCardInfo> tempList = new();
        thisCol.ForEach(thisCard =>
        {
            var newCard = new MonasteryCardInfo();
            newCard.Populate(thisCard.Temp); //hopefully this works.
            newCard.Deck = thisCard.Deck;
            tempList.Add(newCard);
        });
        HandList.ReplaceRange(tempList);
    }
    public bool IsDoubleRun => _setType == EnumMonasterySets.DoubleRun;
    public DeckRegularDict<MonasteryCardInfo> NeedList(int position)
    {
        var thisCard = HandList.First();
        if (thisCard.Value == EnumRegularCardValueList.LowAce && _setType == EnumMonasterySets.DoubleRun)
        {
            thisCard = HandList[1];
        }
        var entireList = _gameContainer.Rummys!.EntireList;
        DeckRegularDict<MonasteryCardInfo> output = new();
        if (_setType == EnumMonasterySets.RegularSuits)
        {
            output.AddRange(entireList.Where(items => items.Suit == thisCard.Suit));
            return output;
        }
        if (_setType == EnumMonasterySets.EvenOdd)
        {
            var temps = thisCard.Value.Value;
            bool isOdd = temps.IsNumberOdd();
            output.AddRange(entireList.Where(items =>
            {
                if (items.Value == EnumRegularCardValueList.HighAce || items.Value == EnumRegularCardValueList.LowAce)
                {
                    return true;
                }
                var xx = items.Value.Value;
                return xx.IsNumberOdd() == isOdd;
            }));
            return output;
        }
        var numberNeeded = thisCard.Value.Value;
        var colorNeeded = thisCard.Color;
        if (_setType == EnumMonasterySets.KindColor)
        {
            output.AddRange(entireList.Where(items =>
            {
                if (items.Color != colorNeeded)
                {
                    return false;
                }
                var temps = items.Value.Value;
                if (temps == numberNeeded)
                {
                    return true;
                }
                return items.Value == EnumRegularCardValueList.HighAce || items.Value == EnumRegularCardValueList.LowAce;
            }));
            return output;
        }
        if (_setType == EnumMonasterySets.RegularKinds) //colors don't matter this time.
        {
            output.AddRange(entireList.Where(items =>
            {
                var temps = items.Value.Value;
                if (temps == numberNeeded)
                {
                    return true;
                }
                return items.Value == EnumRegularCardValueList.HighAce || items.Value == EnumRegularCardValueList.LowAce;
            }));
            return output;
        }
        void SendRunOutput(int needs)
        {
            output.AddRange(entireList.Where(items =>
            {
                if (_setType == EnumMonasterySets.SuitRuns && items.Suit != thisCard.Suit)
                {
                    return false;
                }
                if (_setType == EnumMonasterySets.RunColors && items.Color != thisCard.Color)
                {
                    return false;
                }
                var temps = items.Value.Value;
                if (temps == needs)
                {
                    return true;
                }
                return items.Value == EnumRegularCardValueList.HighAce || items.Value == EnumRegularCardValueList.LowAce;
            })); //don't return yet.
        }
        if (numberNeeded > 1 && numberNeeded < 14 && position == 1)
        {
            numberNeeded--;
            SendRunOutput(numberNeeded);
        }
        thisCard = HandList.Last();
        numberNeeded = thisCard.Value.Value;
        if (numberNeeded < 13 && position == 2)
        {
            numberNeeded++;
            SendRunOutput(numberNeeded);
        }
        return output;
    }
    private MonasteryCardInfo GetCard(MonasteryCardInfo thisCard, int position)
    {
        var tempCard = HandList.First();
        if (tempCard.Value == EnumRegularCardValueList.LowAce && _setType == EnumMonasterySets.DoubleRun)
        {
            tempCard = HandList[1];
        }
        if (_setType == EnumMonasterySets.RegularSuits)
        {
            return thisCard;
        }
        if (thisCard.Value != EnumRegularCardValueList.HighAce && thisCard.Value != EnumRegularCardValueList.LowAce)
        {
            return thisCard;
        }
        if (position == 1)
        {
            if (thisCard.Value == EnumRegularCardValueList.Two)
            {
                if (_setType == EnumMonasterySets.RegularRuns || _setType == EnumMonasterySets.SuitRuns || _setType == EnumMonasterySets.RunColors)
                {
                    return thisCard;
                }
            }
        }
        var entireList = _gameContainer.Rummys!.EntireList;
        if (_setType == EnumMonasterySets.RegularKinds | _setType == EnumMonasterySets.KindColor & tempCard.Color == EnumRegularColorList.Red)
        {
            return entireList.First(items => items.Value == tempCard.Value && items.Suit == EnumSuitList.Diamonds);
        }
        if (_setType == EnumMonasterySets.KindColor)
        {
            return entireList.First(items => items.Value == tempCard.Value && items.Suit == EnumSuitList.Clubs);
        }
        if (_setType == EnumMonasterySets.EvenOdd)
        {
            int nums = tempCard.Value.Value;
            if (nums.IsNumberOdd())
            {
                return thisCard;
            }
            return entireList.First(items => items.Value == tempCard.Value && items.Suit == EnumSuitList.Diamonds);
        }
        if (position == 2)
        {
            tempCard = HandList.Last();
        }
        if (_setType == EnumMonasterySets.DoubleRun)
        {
            int nums = HandList.Count;
            if (nums.IsNumberOdd())
            {
                if (tempCard.Value == EnumRegularCardValueList.LowAce || tempCard.Value == EnumRegularCardValueList.HighAce)
                {
                    return thisCard;
                }
                return entireList.First(items => items.Value == tempCard.Value && items.Suit == EnumSuitList.Diamonds);
            }
        }
        int numberNeeded;
        if (position == 2)
        {
            numberNeeded = tempCard.Value.Value + 1;
        }
        else
        {
            numberNeeded = tempCard.Value.Value - 1;
        }
        if (numberNeeded == 1)
            return thisCard;
        EnumSuitList suitNeeded;
        if (_setType == EnumMonasterySets.RegularRuns)
        {
            suitNeeded = EnumSuitList.Diamonds;
        }
        else if (_setType == EnumMonasterySets.SuitRuns)
        {
            suitNeeded = tempCard.Suit;
        }
        else if (tempCard.Color == EnumRegularColorList.Red)
        {
            suitNeeded = EnumSuitList.Diamonds;
        }
        else
        {
            suitNeeded = EnumSuitList.Clubs;
        }
        return entireList.First(items => items.Value.Value == numberNeeded && items.Suit == suitNeeded);
    }
    public int PositionToPlay(MonasteryCardInfo thisCard, int thisPos)
    {
        var thisList = NeedList(thisPos);
        if (thisList.Count > 0)
        {
            if (thisList.ObjectExist(thisCard.Deck))
            {
                return thisPos;
            }
        }
        if (_setType == EnumMonasterySets.KindColor || _setType == EnumMonasterySets.RegularKinds || _setType == EnumMonasterySets.RegularSuits || _setType == EnumMonasterySets.EvenOdd)
        {
            return 0;
        }
        if (thisPos == 1)
        {
            thisPos = 2;
        }
        else
        {
            thisPos = 1;
        }
        thisList = NeedList(thisPos);
        if (thisList.Count > 0)
        {
            if (thisList.ObjectExist(thisCard.Deck))
            {
                return thisPos;
            }
        }
        return 0;
    }
    public void AddCard(MonasteryCardInfo thisCard, int position)
    {
        var newCard = GetCard(thisCard, position);
        var finalCard = new MonasteryCardInfo();
        finalCard.Populate(newCard.Deck);
        finalCard.Deck = thisCard.Deck;
        if (position == 1)
        {
            HandList.InsertBeginning(finalCard);
        }
        else
        {
            HandList.Add(finalCard);
        }
    }
}