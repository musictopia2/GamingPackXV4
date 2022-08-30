namespace Chinazo.Core.Logic;
public class PhaseSet : SetInfo<EnumSuitList, EnumRegularColorList, ChinazoCard, SavedSet>
{
    private EnumRummyType _whatSet;
    private bool _useSecond;
    private int _firstNumber;
    private readonly ChinazoGameContainer _gameContainer;
    public PhaseSet(ChinazoGameContainer gameContainer) : base(gameContainer.Command)
    {
        CanExpandRuns = true;
        _gameContainer = gameContainer;
    }
    public override void LoadSet(SavedSet payLoad)
    {
        HandList.ReplaceRange(payLoad.CardList);
        _whatSet = payLoad.WhatSet;
        _useSecond = payLoad.UseSecond;
        _firstNumber = payLoad.FirstNumber;
    }
    public override SavedSet SavedSet()
    {
        SavedSet output = new()
        {
            CardList = HandList.ToRegularDeckDict(),
            UseSecond = _useSecond,
            WhatSet = _whatSet
        };
        if (_firstNumber > 12 && _whatSet == EnumRummyType.Sets)
        {
            throw new CustomBasicException("The first number when doing sets can never be higher than 12 when doing autoresume");
        }
        output.FirstNumber = _firstNumber;
        return output;
    }
    protected override bool IsRun()
    {
        return _whatSet == EnumRummyType.Runs;
    }
    private void RepopulateCards()
    {
        if (_gameContainer.ModifyCards == null)
        {
            throw new CustomBasicException("Nobody is handling modify cards.  Rethink");
        }
        int x = _firstNumber;
        _gameContainer.ModifyCards.Invoke(HandList);
        EnumSuitList suitUsed = HandList.First(items => items.IsObjectWild == false).Suit;
        HandList.ForEach(thisCard =>
        {
            thisCard.Suit = suitUsed;
            thisCard.UsedAs = x;
            x++;
        });
    }
    public void CreateSet(IDeckDict<ChinazoCard> thisList, EnumRummyType whichSet, bool useSecond)
    {
        _whatSet = whichSet;
        _useSecond = useSecond;
        var wildCol = thisList.Where(items => items.IsObjectWild == true).ToRegularDeckDict();
        thisList.ForEach(thisCard =>
        {
            thisCard.IsSelected = false;
            thisCard.Drew = false;
        });
        if (_whatSet != EnumRummyType.Runs)
        {
            HandList.ReplaceRange(thisList);
            if (HandList.Count == 0)
            {
                throw new CustomBasicException("the hand list was blank");
            }
            return;
        }
        EnumSuitList suitOfSet = thisList.First(items => items.IsObjectWild == false).Suit;
        int originalNumber = thisList.Count;
        var tempCol = thisList.Where(items => items.IsObjectWild == false).ToRegularDeckDict();
        if (useSecond == true)
        {
            tempCol = tempCol.OrderBy(items => items.SecondNumber).ToRegularDeckDict();
        }
        int firstNum;
        if (useSecond == true)
        {
            tempCol.First().UsedAs = tempCol.First().SecondNumber.Value;
        }
        else
        {
            tempCol.First().UsedAs = tempCol.First().Value.Value;
        }
        firstNum = tempCol.First().UsedAs;
        if (firstNum > 12)
        {
            throw new CustomBasicException("The first number cannot be higher than 12 for runs.");
        }
        tempCol.Last().UsedAs = tempCol.Last().Value.Value;
        int whatFirst = firstNum;
        int lastNum = tempCol.Last().UsedAs;
        int y = tempCol.Count;
        ChinazoCard tempCard;
        for (int x = 2; x <= y; x++)
        {
            firstNum += 1;
            tempCard = tempCol[x - 1];
            if (tempCard.Value.Value != firstNum)
            {
                tempCard = wildCol.First();
                tempCard.UsedAs = firstNum;
                tempCard.Suit = suitOfSet;
                tempCol.Add(tempCard);
                wildCol.RemoveSpecificItem(tempCard);
                x--;
            }
            else
            {
                if (useSecond == true)
                {
                    tempCard.UsedAs = tempCard.SecondNumber.Value;
                }
                else
                {
                    tempCard.UsedAs = tempCard.Value.Value;
                }
            }
            if (tempCard.UsedAs > 14)
            {
                throw new CustomBasicException("The use as cannot be higher than 14 ever");
            }
        }
        if (wildCol.Count > 0)
        {
            lastNum += 1;
            for (int x = lastNum; x <= 14; x++)
            {
                if (wildCol.Count == 0)
                {
                    break;
                }
                tempCard = wildCol.First();
                tempCard.UsedAs = x;
                tempCard.Suit = suitOfSet;
                tempCol.Add(tempCard);
                wildCol.RemoveSpecificItem(tempCard);
            }
            whatFirst--;
            for (int x = whatFirst; x >= 1; x += -1)
            {
                if (wildCol.Count == 0)
                {
                    break;
                }
                tempCard = wildCol.First();
                tempCard.UsedAs = x;
                tempCard.Suit = suitOfSet;
                tempCol.Add(tempCard);
                wildCol.RemoveSpecificItem(tempCard);
            }
            if (tempCol.Count != originalNumber)
            {
                throw new CustomBasicException("Must have the same number of cards sent for creating set");
            }
        }
        if (tempCol.Any(items => items.UsedAs == 0))
        {
            throw new CustomBasicException("You must have accounted for all used.  Rethink");
        }
        var tempList = tempCol.OrderBy(items => items.UsedAs).ToRegularDeckDict();
        HandList.ReplaceRange(tempList);
        if (HandList.Count == 0)
        {
            throw new CustomBasicException("HandList Blank");
        }
        _firstNumber = HandList.First().UsedAs;
    }
    public void CheckList()
    {
        if (HandList.Count == 0)
        {
            throw new CustomBasicException("hand list cannot be blank when double checking");
        }
    }
    public void AddCard(ChinazoCard thisCard, int position)
    {
        thisCard.IsSelected = false;
        thisCard.Drew = false;
        int newPosition = PositionToPlay(thisCard, position);
        if (newPosition == 1)
        {
            if (_whatSet == EnumRummyType.Runs)
            {
                if (thisCard.IsObjectWild == true)
                {
                    thisCard.UsedAs = HandList.First().UsedAs - 1;
                }
                if (thisCard.UsedAs == 14)
                {
                    throw new CustomBasicException("Ace cannot be used as 14 when its low.");
                }
                _firstNumber = thisCard.UsedAs;
            }
            HandList.InsertBeginning(thisCard);
        }
        else
        {
            if (_whatSet == EnumRummyType.Runs)
            {
                if (thisCard.IsObjectWild == true)
                {
                    thisCard.UsedAs = HandList.Last().UsedAs + 1;
                    if (thisCard.UsedAs > 14)
                    {
                        throw new CustomBasicException("Use as can never be higher than 14 ever");
                    }
                }
            }
            HandList.Add(thisCard);
        }
        if (_whatSet == EnumRummyType.Runs)
        {
            RepopulateCards();
        }
    }
    public int PositionToPlay(ChinazoCard thisCard, int position)
    {
        if (thisCard.IsObjectWild == true)
        {
            if (_whatSet != EnumRummyType.Runs)
            {
                return position;
            }
        }
        var newCard = CardNeeded(position);
        if (_whatSet == EnumRummyType.Sets)
        {
            if (newCard.Value == thisCard.Value)
            {
                return position;
            }
            return 0;
        }
        if (newCard.Deck > 0 && thisCard.IsObjectWild == true)
        {
            return position;
        }
        if (position == 1)
        {
            thisCard.UsedAs = thisCard.SecondNumber.Value;
            newCard.UsedAs = newCard.SecondNumber.Value;
        }
        else
        {
            thisCard.UsedAs = thisCard.Value.Value;
            newCard.UsedAs = newCard.Value.Value;
        }
        if (thisCard.Suit == newCard.Suit && thisCard.UsedAs == newCard.UsedAs)
        {
            return position;
        }
        if (position == 1)
        {
            position = 2;
        }
        else
        {
            position = 1;
        }
        newCard = CardNeeded(position);
        if (position == 1)
        {
            thisCard.UsedAs = thisCard.SecondNumber.Value;
            newCard.UsedAs = newCard.SecondNumber.Value;
        }
        else
        {
            thisCard.UsedAs = thisCard.Value.Value;
            newCard.UsedAs = newCard.Value.Value;
        }
        if (newCard.Deck > 0 && thisCard.IsObjectWild == true)
        {
            return position;
        }
        if (thisCard.Suit == newCard.Suit && thisCard.UsedAs == newCard.UsedAs)
        {
            return position;
        }
        return 0;
    }
    public ChinazoCard CardNeeded(int position)
    {
        ChinazoCard? thisCard = null;
        if (position == 1 || _whatSet == EnumRummyType.Sets)
        {
            if (_whatSet == EnumRummyType.Sets)
            {
                thisCard = HandList.First(items => items.IsObjectWild == false);
            }
            else
            {
                thisCard = HandList.First();
            }
        }
        else
        {
            thisCard = HandList.Last();
        }
        if (_whatSet == EnumRummyType.Sets)
        {
            return thisCard;
        }
        if (position == 1)
        {
            if (thisCard.Value == EnumRegularCardValueList.HighAce || thisCard.Value == EnumRegularCardValueList.LowAce)
            {
                return new ChinazoCard(); //because you are finished
            }
            if (thisCard.Value == EnumRegularCardValueList.Two)
            {
                return _gameContainer.DeckList.First(items => items.SecondNumber == EnumRegularCardValueList.LowAce && items.Suit == thisCard.Suit);
            }
            return _gameContainer.DeckList.First(items => items.Value.Value == thisCard.UsedAs - 1 && items.Suit == thisCard.Suit);
        }
        if (thisCard.Value == EnumRegularCardValueList.HighAce || thisCard.Value == EnumRegularCardValueList.LowAce)
        {
            return new ChinazoCard();
        }
        return _gameContainer.DeckList.First(items => items.Value.Value == thisCard.UsedAs + 1 && items.Suit == thisCard.Suit);
    }
}