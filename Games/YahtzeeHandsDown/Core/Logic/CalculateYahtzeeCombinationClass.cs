namespace YahtzeeHandsDown.Core.Logic;
public class CalculateYahtzeeCombinationClass
{
    private int _firstNumber;
    private int _secondNumber;
    private bool HasFullHouse(BasicList<ICard> cardList)
    {
        int x;
        BasicList<ICard> tempList = new();
        for (x = 6; x >= 1; x += -1)
        {

            for (var y = 1; y <= 3; y++)
            {
                var thisColor = y;

                if (_firstNumber == 0)
                {
                    BasicList<ICard> newList = new();
                    tempList = cardList.ToBasicList();
                    do
                    {
                        if (newList.Count == 3)
                        {
                            break;
                        }
                        var thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)thisColor && items.FirstValue == x && items.SecondValue == 0);
                        if (thisCard != null)
                        {
                            newList.Add(thisCard);
                            tempList.RemoveSpecificItem(thisCard);
                        }
                        else
                        {
                            thisCard = tempList.FirstOrDefault(items => items.Color == EnumColor.Any && items.FirstValue == x);
                            if (thisCard != null)
                            {
                                newList.Add(thisCard);
                                tempList.RemoveSpecificItem(thisCard);
                            }
                            else
                            {
                                thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)thisColor && items.FirstValue == x);
                                if (thisCard != null)
                                {
                                    newList.Add(thisCard);
                                    tempList.RemoveSpecificItem(thisCard);
                                }
                                else
                                {
                                    thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)thisColor && items.SecondValue == x);
                                    if (thisCard != null)
                                    {
                                        newList.Add(thisCard);
                                        tempList.RemoveSpecificItem(thisCard);
                                    }
                                    else
                                    {
                                        thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)thisColor && items.IsWild == true);
                                        if (thisCard != null)
                                        {
                                            newList.Add(thisCard);
                                            tempList.RemoveSpecificItem(thisCard);
                                        }
                                        else
                                        {
                                            break; //because no more left.
                                        }
                                    }
                                }
                            }
                        }
                    } while (true);
                    if (newList.Count == 3)
                    {
                        _firstNumber = x;
                        break;
                    }
                }
            }
        }
        if (_firstNumber == 0)
        {
            return false;
        }
        var finList = tempList.ToBasicList();
        for (x = 6; x >= 1; x += -1)
        {

            for (var y = 1; y <= 3; y++)
            {
                var thisColor = y;
                if (_secondNumber == 0)
                {
                    BasicList<ICard> newList = new();
                    tempList = finList.ToBasicList();
                    do
                    {

                        var thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)thisColor && items.FirstValue == x && items.SecondValue == 0);
                        if (thisCard != null)
                        {
                            newList.Add(thisCard);
                            tempList.RemoveSpecificItem(thisCard);
                        }
                        else
                        {
                            thisCard = tempList.FirstOrDefault(items => items.Color == EnumColor.Any && items.FirstValue == x);
                            if (thisCard != null)
                            {
                                newList.Add(thisCard);
                                tempList.RemoveSpecificItem(thisCard);
                            }
                            else
                            {
                                thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)thisColor && items.FirstValue == x);
                                if (thisCard != null)
                                {
                                    newList.Add(thisCard);
                                    tempList.RemoveSpecificItem(thisCard);
                                }
                                else
                                {
                                    thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)thisColor && items.SecondValue == x);
                                    if (thisCard != null)
                                    {
                                        newList.Add(thisCard);
                                        tempList.RemoveSpecificItem(thisCard);
                                    }
                                    else
                                    {
                                        thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)thisColor && items.IsWild == true);
                                        if (thisCard != null)
                                        {
                                            newList.Add(thisCard);
                                            tempList.RemoveSpecificItem(thisCard);
                                        }
                                        else
                                        {
                                            break; //because no more left.
                                        }
                                    }
                                }
                            }
                        }
                        if (newList.Count == 3)
                        {
                            break;
                        }
                    } while (true);
                    if (newList.Count == 2)
                    {
                        _secondNumber = x;
                        break;
                    }
                }
            }
        }
        return _secondNumber > 0;
    }
    private static bool HasSpecificCard(ICard thisCard, int number, EnumColor color)
    {
        if (thisCard.FirstValue == number && thisCard.Color == color)
        {
            return true;
        }
        if (thisCard.SecondValue == number && thisCard.Color == color)
        {
            return true;
        }
        if (thisCard.FirstValue == number && thisCard.Color == EnumColor.Any)
        {
            return true;
        }
        if (thisCard.IsWild == true && thisCard.Color == color)
        {
            return true;
        }
        return false;
    }
    private static int HowManyFound(BasicList<ICard> cardList, int number, EnumColor color)
    {
        return cardList.Count(items => HasSpecificCard(items, number, color));
    }
    private bool HasKinds(BasicList<ICard> cardList, int howMany)
    {
        for (var x = 6; x >= 1; x += -1)
        {
            for (var y = 1; y <= 3; y++)
            {
                if (HowManyFound(cardList, x, (EnumColor)y) >= howMany)
                {
                    _firstNumber = x;
                    return true;
                }
            }
        }
        return false;
    }
    private static bool HasCompleteStraight(BasicList<int> thisList, BasicList<ICard> cardList)
    {
        int y;
        int z = 0;
        BasicList<ICard> tempList;
        BasicList<ICard> newList = new();
        for (y = 1; y <= 3; y++)
        {
            tempList = cardList.ToBasicList();
            newList = new();
            thisList.ForEach(x =>
            {
                z++;
                var thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)y && items.FirstValue == x && items.SecondValue == 0);
                if (thisCard != null)
                {
                    newList.Add(thisCard);
                    tempList.RemoveSpecificItem(thisCard);
                }
                else
                {
                    thisCard = tempList.FirstOrDefault(items => items.Color == EnumColor.Any && items.FirstValue == x);
                    if (thisCard != null)
                    {
                        newList.Add(thisCard);
                        tempList.RemoveSpecificItem(thisCard);
                    }
                    else
                    {
                        thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)y && items.FirstValue == x);
                        if (thisCard != null)
                        {
                            newList.Add(thisCard);
                            tempList.RemoveSpecificItem(thisCard);
                        }
                        else
                        {
                            thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)y && items.SecondValue == x);
                            if (thisCard != null)
                            {
                                newList.Add(thisCard);
                                tempList.RemoveSpecificItem(thisCard);
                            }
                            else
                            {
                                thisCard = tempList.FirstOrDefault(items => items.Color == (EnumColor)y && items.IsWild == true);
                                if (thisCard != null)
                                {
                                    newList.Add(thisCard);
                                    tempList.RemoveSpecificItem(thisCard);
                                }
                            }
                        }
                    }
                }
            });
            if (newList.Count == thisList.Count)
            {
                return true;
            }
        }
        return false;
    }
    private static BasicList<int> StraightPoints(BasicList<ICard> cardList, int howManyInStraight)
    {
        BasicList<BasicList<int>> tempList = new();
        if (howManyInStraight == 4)
        {
            tempList.Add(new() { 1, 2, 3, 4 });
            tempList.Add(new() { 2, 3, 4, 5 });
            tempList.Add(new() { 3, 4, 5, 6 });
        }
        else
        {
            tempList.Add(new() { 1, 2, 3, 4, 5 });
            tempList.Add(new() { 2, 3, 4, 5, 6 });
        }
        BasicList<int> output = new();
        tempList.ForEach(thisItem =>
        {
            if (HasCompleteStraight(thisItem, cardList))
            {
                output.Add(thisItem.Sum(items => items));
            }
        });
        return output;
    }
    private bool HasStraight(BasicList<ICard> cardList, int howManyInStraight)
    {
        var tempList = StraightPoints(cardList, howManyInStraight);
        if (tempList.Count == 0)
        {
            return false;
        }
        _firstNumber = tempList.OrderByDescending(items => items).First();
        return true;
    }
    public BasicList<YahtzeeResults> GetResults(BasicList<ComboCardInfo> combinationList, BasicList<ICard> cardList) // 0 means no results found
    {
        BasicList<YahtzeeResults> resultList = new();
        combinationList.ForEach(thisCombo =>
        {
            _firstNumber = 0;
            _secondNumber = 0;
            var tempList = cardList.ToBasicList();
            if (thisCombo.FirstSet > 0 && thisCombo.SecondSet > 0)
            {
                if (HasFullHouse(tempList) == true)
                {
                    if (_firstNumber == 0 || _secondNumber == 0)
                    {
                        throw new CustomBasicException("Must have a first and second number used");
                    }
                    resultList.Add(new YahtzeeResults() { NumberUsed = _firstNumber + _secondNumber, Points = thisCombo.Points });
                }
            }
            else if (thisCombo.FirstSet > 0)
            {
                if (HasKinds(tempList, thisCombo.FirstSet) == true)
                {
                    if (_firstNumber == 0)
                    {
                        throw new CustomBasicException("Must have a number used for the kinds");
                    }
                    resultList.Add(new YahtzeeResults() { NumberUsed = _firstNumber, Points = thisCombo.Points });
                }
            }
            else if (thisCombo.NumberInStraight > 0)
            {
                if (HasStraight(tempList, thisCombo.NumberInStraight) == true)
                {
                    resultList.Add(new YahtzeeResults() { NumberUsed = _firstNumber, Points = thisCombo.Points });
                }
            }
            else
            {
                throw new Exception("Invalid Combo");
            }
        });
        resultList = (from items in resultList
                      orderby items.Points descending, items.NumberUsed descending
                      select items).ToBasicList();
        return resultList;
    }
    public static int WhoWon(BasicList<YahtzeeResults> resultList) // either 1 or 2.
    {
        if (resultList.Count == 1)
        {
            return 1;// can be only 1
        }
        if (resultList.Count != 2)
        {
            throw new CustomBasicException("Must have 2 items to show the best hands from the players; not " + resultList.Count);
        }
        if (resultList.First().Points > resultList.Last().Points)
        {
            return 1;
        }
        if (resultList.Last().Points > resultList.First().Points)
        {
            return 2;
        }
        if (resultList.First().NumberUsed > resultList.Last().NumberUsed)
        {
            return 1;
        }
        if (resultList.Last().NumberUsed > resultList.First().NumberUsed)
        {
            return 2;
        }
        return 1;
    }
}
