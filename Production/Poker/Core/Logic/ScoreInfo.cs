namespace Poker.Core.Logic;
public class ScoreInfo
{
    public DeckRegularDict<PokerCardInfo>? CardList;
    public bool IsRoyalFlush()
    {
        if (CardList!.Any(items => items.Value == EnumRegularCardValueList.HighAce) == false)
        {
            return false;
        }
        return IsStraightFlush();
    }
    public bool IsStraightFlush()
    {
        return IsFlush() && IsStraight();
    }
    public bool IsFlush()
    {
        return CardList!.GroupBy(items => items.Suit).Count() == 1;
    }
    public bool Kinds(int howMany)
    {
        var thisList = CardList!.GroupOrderDescending(items => items.Value).ToBasicList();
        return thisList.Any(items => items.Count() == howMany);
    }
    public bool IsFullHouse()
    {
        var thisList = CardList!.GroupOrderDescending(items => items.Value).ToBasicList();
        if (thisList.Count != 2)
        {
            return false;
        }
        return thisList.First().Count() == 3;
    }
    public bool IsStraight()
    {
        bool acess;
        acess = HasAce();
        int y;
        IEnumerable<PokerCardInfo> sortList;
        bool straightSoFar;
        int currentNumber;
        int previousNumber;
        for (y = 1; y <= 2; y++)
        {
            if (y == 1)
            {
                sortList = from Cards in CardList
                           orderby Cards.Value
                           select Cards;
            }
            else
            {
                sortList = from Cards in CardList
                           orderby Cards.SecondNumber
                           select Cards;
            }
            straightSoFar = true;
            currentNumber = 0;
            previousNumber = 0;
            foreach (var thisCard in sortList)
            {
                if (previousNumber == 0)
                {
                    if (y == 1)
                    {
                        previousNumber = thisCard.Value.Value;
                    }
                    else
                    {
                        previousNumber = thisCard.SecondNumber.Value;
                    }
                }
                else if (y == 1)
                {
                    currentNumber = thisCard.Value.Value;
                }
                else
                {
                    currentNumber = thisCard.SecondNumber.Value;
                }
                if ((currentNumber > 0) & (previousNumber > 0))
                {
                    if ((previousNumber + 1) != currentNumber)
                    {
                        straightSoFar = false;
                        break;
                    }
                }
            }
            if (straightSoFar == true)
            {
                return true;
            }
            if (acess == false)
            {
                return false;
            }
        }
        return false;
    }
    public bool MultiPair()
    {
        var thisList = CardList!.GroupOrderDescending(items => items.Value).ToBasicList();
        if (thisList.Count != 3)
        {
            return false;
        }
        return thisList.First().Count() == 2 && thisList[1].Count() == 2;
    }
    public bool HasAce()
    {
        return CardList!.Any(items => items.Value == EnumRegularCardValueList.HighAce);
    }
}