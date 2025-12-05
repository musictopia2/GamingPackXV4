namespace MonasteryCardGame.Core.Logic;
public class RummyClass
{
    public DeckRegularDict<MonasteryCardInfo> EntireList = new();
    public RummyClass(MonasteryCardGameGameContainer gameContainer)
    {
        EntireList = gameContainer.DeckList!.ToRegularDeckDict();
        if (EntireList.Count == 0)
        {
            throw new CustomBasicException("Cannot have an empty list.  Rethink");
        }
    }
    private static DeckRegularDict<MonasteryCardInfo> PopulateTempCol(DeckRegularDict<MonasteryCardInfo> thisCol, out DeckRegularDict<MonasteryCardInfo> aceList)
    {
        aceList = thisCol.Where(items => items.Value == EnumRegularCardValueList.LowAce || items.Value == EnumRegularCardValueList.HighAce).ToRegularDeckDict();
        DeckRegularDict<MonasteryCardInfo> output = thisCol.ToRegularDeckDict();
        output.RemoveGivenList(aceList);
        return output;
    }
    public static bool IsDoubleRun(DeckRegularDict<MonasteryCardInfo> thisCol)
    {
        if (thisCol.Count < 6)
        {
            return false; //must have at least 3 cards to even be considered for this.
        }
        int counts = thisCol.Count;
        if (counts.IsNumberOdd == true)
        {
            return false; //has to be even number to even be considered.
        }
        var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
        int y;
        int z = 0;
        int q = 0;
        int currentNumber = 0;
        int diffs;
        int previousNumber = 0;
        int removes = 0;
        MonasteryCardInfo thisCard;
        do
        {
            for (y = 1; y <= 2; y++)
            {
                z++;
                q++;
                if (q > thisCol.Count)
                {
                    return true; //if went far enough, then its fine.
                }
                if (z > tempCol.Count) //maybe it was supposed to be tempcol here.
                {
                    diffs = thisCol.Count - tempCol.Count;
                    return aceList.Count + removes == diffs;
                }
                thisCard = tempCol[z - 1];
                if (q == 1)
                {
                    currentNumber = thisCard.Value.Value;
                    previousNumber = thisCard.Value.Value;
                }
                if (y == 2)
                {
                    if (thisCard.Value.Value > currentNumber) //see if it matches
                    {
                        if (aceList.Count == 0)
                        {
                            return false;
                        }
                        z--; //because needs to look again
                        if (currentNumber + 1 == thisCard.Value.Value)
                        {
                            removes++;
                            aceList.RemoveFirstItem(); //because we had an ace.
                        }
                        else if (currentNumber + 2 == thisCard.Value.Value)
                        {
                            if (aceList.Count < 3)
                            {
                                return false;//because not enough aces
                            }
                            3.Times(x =>
                            {
                                removes++;
                                aceList.RemoveFirstItem();
                            });
                            q += 2;
                            previousNumber = thisCard.Value.Value; //because it has to add 2.
                        }
                        else if (currentNumber + 3 == thisCard.Value.Value)
                        {
                            if (aceList.Count < 5)
                            {
                                return false;
                            }
                            5.Times(x =>
                            {
                                removes++;
                                aceList.RemoveFirstItem();
                            });
                            q += 4;
                            previousNumber = thisCard.Value.Value;
                        }
                        else if (currentNumber + 4 == thisCard.Value.Value)
                        {
                            if (aceList.Count < 7)
                            {
                                return false;
                            }
                            7.Times(x =>
                            {
                                removes++;
                                aceList.RemoveFirstItem();
                            });
                            q += 6;
                            previousNumber = thisCard.Value.Value;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if (q > 1)
                {
                    currentNumber = thisCard.Value.Value;
                    diffs = currentNumber - previousNumber - 1;
                    if (diffs > 0)
                    {
                        z--;
                        if (diffs == 1)
                        {
                            if (aceList.Count < 2)
                            {
                                return false;
                            }
                            2.Times(x =>
                            {
                                removes++;
                                aceList.RemoveFirstItem();
                            });
                            q++;
                        }
                        else if (diffs == 2)
                        {
                            if (aceList.Count < 4)
                            {
                                return false; //i think had to remove 4 and not 2.
                            }
                            4.Times(x =>
                            {
                                removes++;
                                aceList.RemoveFirstItem();
                            });
                            q += 3;
                        }
                        else if (diffs == 3)
                        {
                            if (aceList.Count < 6)
                            {
                                return false; //i think had to remove 4 and not 2.
                            }
                            6.Times(x =>
                            {
                                removes++;
                                aceList.RemoveFirstItem();
                            });
                            q += 5;
                        }
                        else if (diffs == 4)
                        {
                            if (aceList.Count < 8)
                            {
                                return false; //i think had to remove 4 and not 2.
                            }
                            8.Times(x =>
                            {
                                removes++;
                                aceList.RemoveFirstItem();
                            });
                            q += 7;
                        }
                    }
                }
                previousNumber = currentNumber;
            }
        } while (true);
    }
    public static bool IsRun(DeckRegularDict<MonasteryCardInfo> thisCol, EnumRunType needType, int required)
    {
        if (thisCol.Count < required)
        {
            return false;
        }
        if (needType == EnumRunType.Suit)
        {
            if (thisCol.GroupBy(items => items.Suit).Count() > 1)
            {
                return false;
            }
        }
        if (needType == EnumRunType.Color)
        {
            if (thisCol.GroupBy(items => items.Color).Count() > 1)
            {
                return false;
            }
        }
        var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
        if (tempCol.Count == 0)
        {
            return true; //if all aces, then fine at this point.
        }
        int howMany = tempCol.GroupBy(items => items.Value).Count();
        if (howMany != tempCol.Count)
        {
            return false;
        }
        int currentNumber = 0;
        int diffs;
        int previousNumber = 0;
        int x = 0;
        foreach (var thisCard in tempCol)
        {
            x++;
            currentNumber = thisCard.Value.Value;
            if (x == 1)
            {
                previousNumber = thisCard.Value.Value;
            }
            else
            {
                diffs = currentNumber - previousNumber - 1;
                if (diffs > 0)
                {
                    if (aceList.Count < diffs)
                    {
                        return false;
                    }
                    diffs.Times(y => aceList.RemoveFirstItem());
                }
            }
            previousNumber = thisCard.Value.Value;
        }
        return true;
    }
    public static bool IsColor(DeckRegularDict<MonasteryCardInfo> thisCol, int required)
    {
        if (thisCol.Count < required)
        {
            return false;
        }
        return thisCol.GroupBy(items => items.Color).Count() == 1;
    }
    public static bool IsKind(DeckRegularDict<MonasteryCardInfo> thisCol, bool needColor, int required)
    {
        if (thisCol.Count < required)
        {
            return false;
        }
        if (needColor)
        {
            if (thisCol.GroupBy(items => items.Color).Count() > 1)
            {
                return false;
            }
        }
        var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
        if (tempCol.Count == 0)
        {
            return true;
        }
        return tempCol.GroupBy(items => items.Value).Count() == 1;
    }
    public static bool IsEvenOdd(DeckRegularDict<MonasteryCardInfo> thisCol)
    {
        if (thisCol.Count < 9)
        {
            return false;
        }
        var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
        if (tempCol.Count == 0)
        {
            return true; //if you somehow was able to get 9 aces, then its okay.
        }
        int thisValue = tempCol.First().Value.Value;
        bool isOdd = thisValue.IsNumberOdd;
        return tempCol.All(items =>
        {
            thisValue = items.Value.Value;
            return thisValue.IsNumberOdd == isOdd;
        });
    }
    public static bool IsSuit(DeckRegularDict<MonasteryCardInfo> thisCol, int required)
    {
        if (thisCol.Count < required)
        {
            return false;
        }
        return thisCol.GroupBy(items => items.Suit).Count() == 1;
    }
    public static DeckRegularDict<MonasteryCardInfo> DoubleRunList(DeckRegularDict<MonasteryCardInfo> thisCol)
    {
        return thisCol;
    }
    public DeckRegularDict<MonasteryCardInfo> RunList(DeckRegularDict<MonasteryCardInfo> thisCol, EnumRunType runType)
    {
        var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
        DeckRegularDict<MonasteryCardInfo> output = new();
        var thisCard = tempCol.First();
        int minNumber = thisCard.Value.Value;
        int maxNumber = minNumber + thisCol.Count - 1;
        if (maxNumber > 13)
        {
            thisCard = tempCol.Last();
            maxNumber = thisCard.Value.Value;
            minNumber = maxNumber - thisCol.Count + 1;
        }
        EnumSuitList suitNeeded;
        if (runType == EnumRunType.Color)
        {
            if (thisCard.Color == EnumRegularColorList.Red)
            {
                suitNeeded = EnumSuitList.Diamonds;
            }
            else
            {
                suitNeeded = EnumSuitList.Clubs;
            }
        }
        else if (runType == EnumRunType.Suit)
        {
            suitNeeded = thisCard.Suit;
        }
        else
        {
            suitNeeded = EnumSuitList.Diamonds;
        }
        int currentNum = minNumber;
        thisCol.Count.Times(x =>
        {
            if (tempCol.Any(items => items.Value.Value == currentNum) == false)
            {
                thisCard = EntireList.First(items => items.Suit == suitNeeded && items.Value.Value == currentNum);
                var nextCard = new MonasteryCardInfo();
                nextCard.Populate(thisCard.Deck);
                nextCard.Temp = thisCard.Deck; //this too i think.
                nextCard.Deck = aceList.First().Deck;//this is what i had to do now.
                aceList.RemoveFirstItem(); //has to use the deck of the ace to stop the id problems.
                output.Add(nextCard);
            }
            else
            {
                thisCard = tempCol.Single(items => items.Value.Value == currentNum);
                tempCol.RemoveSpecificItem(thisCard);
                output.Add(thisCard);
            }
            currentNum++;
        });
        return output;
    }
    public DeckRegularDict<MonasteryCardInfo> KindList(DeckRegularDict<MonasteryCardInfo> thisCol, bool needColor)
    {
        var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
        DeckRegularDict<MonasteryCardInfo> output = new();
        if (aceList.Count == 0)
        {
            return thisCol;
        }
        if (tempCol.Count == 0)
        {
            return aceList;
        }
        var thisCard = tempCol.First();
        int numberNeeded = thisCard.Value.Value;
        EnumSuitList suitNeeded;
        if (needColor)
        {
            if (thisCard.Color == EnumRegularColorList.Red)
            {
                suitNeeded = EnumSuitList.Diamonds;
            }
            else
            {
                suitNeeded = EnumSuitList.Clubs;
            }
        }
        else
        {
            suitNeeded = EnumSuitList.Diamonds;
        }
        aceList.Count.Times(x =>
        {
            thisCard = EntireList.First(items => items.Suit == suitNeeded && items.Value.Value == numberNeeded);
            var nextCard = new MonasteryCardInfo();
            nextCard.Populate(thisCard.Deck); //needs to clone it.
            nextCard.Temp = thisCard.Deck;
            nextCard.Deck = aceList[x - 1].Deck;//this is what i had to do now.
            tempCol.Add(nextCard);
        });
        return tempCol;
    }
    public DeckRegularDict<MonasteryCardInfo> EvenOddList(DeckRegularDict<MonasteryCardInfo> thisCol)
    {
        var tempCol = PopulateTempCol(thisCol, out DeckRegularDict<MonasteryCardInfo> aceList);
        DeckRegularDict<MonasteryCardInfo> output = new();
        if (aceList.Count == 0)
        {
            return thisCol;
        }
        if (tempCol.Count == 0)
        {
            return aceList;
        }
        var thisCard = tempCol.First();
        int numberNeeded = thisCard.Value.Value;
        aceList.Count.Times(x =>
        {
            thisCard = EntireList.First(items => items.Suit == EnumSuitList.Diamonds && items.Value.Value == numberNeeded);
            var nextCard = new MonasteryCardInfo();
            nextCard.Populate(thisCard.Deck);
            nextCard.Temp = thisCard.Deck;
            nextCard.Deck = aceList[x - 1].Deck; //to stop the linking problems.
            tempCol.Add(nextCard);
        });
        return tempCol;
    }
}
