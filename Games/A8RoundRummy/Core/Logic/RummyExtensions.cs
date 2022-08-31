namespace A8RoundRummy.Core.Logic;
public static class RummyExtensions
{
    private static A8RoundRummyCardInformation GetLastCard(this IDeckDict<A8RoundRummyCardInformation> originalList, IDeckDict<A8RoundRummyCardInformation> newList)
    {
        if (originalList.Count != 8 && newList.Count != 7)
        {
            throw new CustomBasicException("The original list must have 8 cards and the new list must have 7 cards");
        }
        return originalList.Single(card =>
        {
            return newList.ObjectExist(card.Deck) == false;
        });
    }
    public static bool HasRummy(this IDeckDict<A8RoundRummyCardInformation> cardList, A8RoundRummyMainGameClass mainGame) //you have to send one more argument
    {
        if (cardList.Count != 8)
        {
            throw new CustomBasicException("Must have 8 cards in order to have the rummy");
        }
        mainGame.LastCard = null;
        mainGame.LastSuccessful = false;
        RoundClass currentRound = mainGame.SaveRoot!.RoundList.First();
        int wilds = cardList.Count(xx => xx.CardType == EnumCardType.Wild);
        int reverses = cardList.Count(xx => xx.CardType == EnumCardType.Reverse);
        EnumCardShape shapeUsed;
        EnumColor colorUsed;
        if (reverses > 1)
        {
            return false; //can't have more than one reverse in your hand.
        }
        if (currentRound.Category == EnumCategory.Colors && currentRound.Rummy == EnumRummyType.Regular)
        {
            var blueList = cardList.Where(xx => xx.Color == EnumColor.LightBlue || xx.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
            var redList = cardList.Where(xx => xx.Color == EnumColor.Red || xx.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
            if (blueList.Count < 7 && redList.Count < 7) //has to be and, not or.
            {
                return false;
            }
            mainGame.LastSuccessful = true;
            if (blueList.Count == 7)
            {
                mainGame.LastCard = cardList.GetLastCard(blueList);
            }
            else
            {
                mainGame.LastCard = cardList.GetLastCard(redList);
            }
            return true;
        }
        DeckRegularDict<A8RoundRummyCardInformation> tempList;
        if (currentRound.Category == EnumCategory.Shapes && currentRound.Rummy == EnumRummyType.Regular)
        {
            var firstList = cardList.Where(xx => xx.CardType == EnumCardType.Regular).GroupBy(Items => Items.Shape).ToBasicList();
            if (firstList.Count > 2)
            {
                return false;
            }
            if (firstList.Count > 1 && reverses > 0)
            {
                return false;
            }
            if (firstList.Count > 1)
            {
                if (firstList.First().Count() > 1 && firstList.Last().Count() > 1)
                {
                    return false;
                }
                if (firstList.First().Count() > 1)
                {
                    shapeUsed = firstList.First().Key;
                }
                else
                {
                    shapeUsed = firstList.Last().Key;
                }
            }
            else
            {
                shapeUsed = firstList.First().Key;
            }
            tempList = cardList.Where(xx => xx.Shape == shapeUsed || xx.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
            mainGame.LastSuccessful = true;
            mainGame.LastCard = cardList.GetLastCard(tempList);
            return true;
        }
        if (currentRound.Category == EnumCategory.Both && currentRound.Rummy == EnumRummyType.Regular)
        {
            var firstList = cardList.Where(xx => xx.CardType == EnumCardType.Regular).GroupBy(Items => new { Items.Color, Items.Shape }).ToBasicList();
            if (firstList.Count > 2)
            {
                return false;
            }
            if (firstList.Count > 1 && reverses > 0)
            {
                return false;
            }
            if (firstList.Count > 1)
            {
                if (firstList.First().Count() > 1 && firstList.Last().Count() > 1)
                {
                    return false;
                }
                if (firstList.First().Count() > 1)
                {
                    shapeUsed = firstList.First().Key.Shape;
                    colorUsed = firstList.First().Key.Color;
                }
                else
                {
                    shapeUsed = firstList.Last().Key.Shape;
                    colorUsed = firstList.Last().Key.Color;
                }
            }
            else
            {
                shapeUsed = firstList.First().Key.Shape;
                colorUsed = firstList.First().Key.Color;
            }
            tempList = cardList.Where(xx => xx.CardType == EnumCardType.Wild || (xx.Color == colorUsed && xx.Shape == shapeUsed)).Take(7).ToRegularDeckDict();
            mainGame.LastSuccessful = true;
            mainGame.LastCard = cardList.GetLastCard(tempList);
            return true;
        }
        if (currentRound.Rummy == EnumRummyType.Kinds)
        {
            var firstList = cardList.Where(xx => xx.CardType == EnumCardType.Regular).GroupBy(xx => xx.Value).ToBasicList();
            int numberUsed;
            if (firstList.Count > 2)
            {
                return false;
            }
            if (firstList.Count > 1 && reverses > 0)
            {
                return false;
            }
            if (firstList.Count > 1)
            {
                if (firstList.First().Count() > 1 && firstList.Last().Count() > 1)
                {
                    return false;
                }
                if (firstList.First().Count() > 1)
                {
                    numberUsed = firstList.First().Key;
                }
                else
                {
                    numberUsed = firstList.Last().Key;
                }
            }
            else
            {
                numberUsed = firstList.First().Key;
            }
            tempList = cardList.Where(xx => xx.Value == numberUsed || xx.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
            mainGame.LastSuccessful = true;
            mainGame.LastCard = cardList.GetLastCard(tempList);
            return true;
        }
        if (currentRound.Rummy != EnumRummyType.Straight)
        {
            throw new CustomBasicException("Not Supported");
        }
        var straightList = cardList.Where(xx => xx.CardType == EnumCardType.Regular).OrderBy(Items => Items.Value).ToRegularDeckDict();
        switch (currentRound.Category)
        {
            case EnumCategory.None:
                var nexts = straightList.GroupBy(xx => xx.Value).ToBasicList();
                var finList = nexts.Where(xx => xx.Count() > 1).ToBasicList();
                if (finList.Count == 0)
                {
                    tempList = cardList.Where(xx => xx.CardType != EnumCardType.Reverse).Take(7).ToRegularDeckDict();
                    mainGame.LastSuccessful = true;
                    mainGame.LastCard = cardList.GetLastCard(tempList);
                    return true;
                }
                if (reverses == 1)
                {
                    return false;
                }
                if (finList.Count > 1)
                {
                    return false;
                }
                mainGame.LastSuccessful = true;
                mainGame.LastCard = finList.Single().First();
                return true;
            case EnumCategory.Colors:
                {
                    return cardList.IsStraightColorOnly(straightList, mainGame, reverses);
                }
            case EnumCategory.Shapes:
                {
                    return cardList.IsStraightShapeOnly(straightList, mainGame, reverses);
                }
            case EnumCategory.Both:
                {
                    return cardList.IsStraightBoth(straightList, mainGame, reverses);
                }
            default:
                throw new CustomBasicException("Not Supported");
        }
    }
    private static bool IsStraightColorOnly(this IDeckDict<A8RoundRummyCardInformation> cardList, IDeckDict<A8RoundRummyCardInformation> straightList, A8RoundRummyMainGameClass mainGame, int reverses)
    {
        var firstList = straightList.GroupBy(xx => xx.Color).ToBasicList();
        if (firstList.Count > 2)
        {
            throw new CustomBasicException("Can only have 2 colors at the most");
        }
        var nextFirst = firstList.First().GroupBy(xx => xx.Value).ToBasicList();
        var finList = nextFirst.Where(xx => xx.Count() > 1).ToBasicList();
        DeckRegularDict<A8RoundRummyCardInformation> tempList;
        EnumColor colorUsed;
        if (firstList.Count == 1)
        {
            if (finList.Count == 0)
            {
                tempList = cardList.Where(xx => xx.CardType != EnumCardType.Reverse).Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(tempList);
                return true;
            }
            if (reverses == 1)
            {
                return false;
            }
            if (finList.Count > 1)
            {
                return false;
            }
            mainGame.LastSuccessful = true;
            mainGame.LastCard = finList.Single().First();
            return true;
        }
        if (reverses == 1)
        {
            return false;
        }
        var nextSecond = firstList.Last().GroupBy(xx => xx.Value).ToBasicList();
        if (nextSecond.Count > 1 && nextFirst.Count > 1)
        {
            return false;
        }
        if (nextSecond.Count > 1)
        {
            colorUsed = firstList.Last().Key;
            finList = nextSecond.Where(xx => xx.Count() > 1).ToBasicList();
        }
        else
        {
            colorUsed = firstList.First().Key;
        }
        if (finList.Count == 0)
        {
            tempList = cardList.Where(xx => xx.Color == colorUsed || xx.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
            mainGame.LastSuccessful = true;
            mainGame.LastCard = cardList.GetLastCard(tempList);
            return true;
        }
        if (finList.Count > 1)
        {
            return false;
        }
        mainGame.LastSuccessful = true;
        mainGame.LastCard = finList.Single().First();
        return true;
    }
    private static bool IsStraightShapeOnly(this IDeckDict<A8RoundRummyCardInformation> cardList, IDeckDict<A8RoundRummyCardInformation> straightList, A8RoundRummyMainGameClass mainGame, int reverses)
    {
        var firstList = straightList.GroupBy(xx => xx.Shape).ToBasicList();
        if (firstList.Count > 2)
        {
            return false;
        }
        var nextFirst = firstList.First().GroupBy(xx => xx.Value).ToBasicList();
        var finList = nextFirst.Where(xx => xx.Count() > 1).ToBasicList();
        DeckRegularDict<A8RoundRummyCardInformation> tempList;
        EnumCardShape shapeUsed;
        if (firstList.Count == 1)
        {
            if (finList.Count == 0)
            {
                tempList = cardList.Where(xx => xx.CardType != EnumCardType.Reverse).Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(tempList);
                return true;
            }
            if (reverses == 1)
            {
                return false;
            }
            if (finList.Count > 1)
            {
                return false;
            }
            mainGame.LastSuccessful = true;
            mainGame.LastCard = finList.Single().First();
            return true;
        }
        if (reverses == 1)
        {
            return false;
        }
        var nextSecond = firstList.Last().GroupBy(xx => xx.Value).ToBasicList();
        if (nextSecond.Count > 1 && nextFirst.Count > 1)
        {
            return false;
        }
        if (nextSecond.Count > 1)
        {
            shapeUsed = firstList.Last().Key;
            finList = nextSecond.Where(xx => xx.Count() > 1).ToBasicList();
        }
        else
        {
            shapeUsed = firstList.First().Key;
        }
        if (finList.Count == 0)
        {
            tempList = cardList.Where(xx => xx.Shape == shapeUsed || xx.CardType == EnumCardType.Wild).Take(7).ToRegularDeckDict();
            mainGame.LastSuccessful = true;
            mainGame.LastCard = cardList.GetLastCard(tempList);
            return true;
        }
        if (finList.Count > 1)
        {
            return false;
        }
        mainGame.LastSuccessful = true;
        mainGame.LastCard = finList.Single().First();
        return true;
    }
    private static bool IsStraightBoth(this IDeckDict<A8RoundRummyCardInformation> cardList, IDeckDict<A8RoundRummyCardInformation> straightList, A8RoundRummyMainGameClass mainGame, int reverses)
    {
        var firstList = straightList.GroupBy(xx => new { xx.Color, xx.Shape }).ToBasicList();
        if (firstList.Count > 2)
        {
            return false;
        }
        var nextFirst = firstList.First().GroupBy(xx => xx.Value).ToBasicList();
        var finList = nextFirst.Where(xx => xx.Count() > 1).ToBasicList();
        DeckRegularDict<A8RoundRummyCardInformation> tempList;
        EnumCardShape shapeUsed;
        EnumColor colorUsed;
        if (firstList.Count == 1)
        {
            if (finList.Count == 0)
            {
                tempList = cardList.Where(xx => xx.CardType != EnumCardType.Reverse).Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(tempList);
                return true;
            }
            if (reverses == 1)
            {
                return false;
            }
            if (finList.Count > 1)
            {
                return false;
            }
            mainGame.LastSuccessful = true;
            mainGame.LastCard = finList.Single().First();
            return true;
        }
        if (reverses == 1)
        {
            return false;
        }
        var nextSecond = firstList.Last().GroupBy(xx => xx.Value).ToBasicList();
        if (nextSecond.Count > 1 && nextFirst.Count > 1)
        {
            return false;
        }
        if (nextSecond.Count > 1)
        {
            shapeUsed = firstList.Last().Key.Shape;
            colorUsed = firstList.Last().Key.Color;
            finList = nextSecond.Where(xx => xx.Count() > 1).ToBasicList();
        }
        else
        {
            shapeUsed = firstList.First().Key.Shape;
            colorUsed = firstList.First().Key.Color;
        }
        if (finList.Count == 0)
        {
            tempList = cardList.Where(xx => xx.CardType == EnumCardType.Wild || (xx.Shape == shapeUsed && xx.Color == colorUsed)).Take(7).ToRegularDeckDict();
            mainGame.LastSuccessful = true;
            mainGame.LastCard = cardList.GetLastCard(tempList);
            return true;
        }
        if (finList.Count > 1)
        {
            return false;
        }
        mainGame.LastSuccessful = true;
        mainGame.LastCard = finList.Single().First();
        return true;
    }
    public static bool GuaranteedVictory(this IDeckDict<A8RoundRummyCardInformation> cardList, A8RoundRummyMainGameClass mainGame)
    {
        mainGame.LastSuccessful = false;
        mainGame.LastCard = null;
        if (mainGame.SaveRoot!.RoundList.Last().Rummy != EnumRummyType.Kinds)
        {
            return false; //only last round has this.
        }
        if (cardList.Count(xx => xx.CardType != EnumCardType.Regular) > 1)
        {
            return false;
        }
        var tempList = cardList.Where(xx => xx.CardType == EnumCardType.Regular).GroupBy(xx => xx.Value).ToBasicList();
        if (tempList.Count == 1)
        {
            if (tempList.First().Count() >= 7)
            {
                var finList = tempList.Single().Take(7).ToRegularDeckDict();
                mainGame.LastSuccessful = true;
                mainGame.LastCard = cardList.GetLastCard(finList);
                return true;
            }
            return false;
        }
        if (tempList.Count > 2)
        {
            return false;
        }
        if (tempList.First().Count() < 7 && tempList.Last().Count() < 7)
        {
            return false;
        }
        if (tempList.First().Count() >= 7)
        {
            var finList = tempList.First().Take(7).ToRegularDeckDict();
            mainGame.LastSuccessful = true;
            mainGame.LastCard = cardList.GetLastCard(finList);
            return true;
        }
        else
        {
            if (tempList.Last().Count() < 7)
            {
                throw new CustomBasicException("Had to have 7");
            }
            var finList = tempList.Last().Take(7).ToRegularDeckDict();
            mainGame.LastSuccessful = true;
            mainGame.LastCard = cardList.GetLastCard(finList);
            return true;
        }
    }
}
