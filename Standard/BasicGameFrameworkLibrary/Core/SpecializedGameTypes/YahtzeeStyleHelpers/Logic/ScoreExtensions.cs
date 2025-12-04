namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
public static class ScoreExtensions
{
    extension (ScoreContainer scoreContainer)
    {
        public bool HasFullHouse()
        {
            var tempList = scoreContainer.DiceList.GroupOrderDescending(items => items.Value);
            if (tempList.Count() == 1)
            {
                return true; //3 and 2 but same number is acceptable.
            }
            if (tempList.Count() != 2)
            {
                return false;
            }
            if (tempList.First().Count() != 3)
            {
                return false;

            }
            return true;
        }
        public bool HasAllFive()
        {
            int count = scoreContainer.DiceList.MaximumDuplicates(items => items.Value);
            return count == 5;
        }
        public bool HasKind(int howMany)
        {
            int count = scoreContainer.DiceList.MaximumDuplicates(items => items.Value);
            return count >= howMany;
        }
        public bool HasStraight(bool smallOnly)
        {
            var tempList = scoreContainer.DiceList.OrderBy(items => items.Value).GroupBy(items => items.Value).ToBasicList();
            if (tempList.Count != 5 && smallOnly == false)
            {
                return false;
            }
            if (tempList.Count < 4)
            {
                return false;
            }
            if (tempList.Count == 5)
            {
                if (tempList.First().Key == 1 && tempList.Last().Key == 6 && smallOnly == false)
                {
                    return false;
                }
                else if (smallOnly == false)
                {
                    return true;
                }
                bool rets = false;
                for (int x = 1; x <= 3; x++)
                {
                    for (int y = x; y <= x + 3; y++)
                    {
                        rets = tempList.Any(Items => Items.Key == y);
                        if (rets == false)
                        {
                            break;
                        }
                    }
                    if (rets == true)
                    {
                        return true;
                    }
                }
                return false;
            }
            for (int x = 1; x <= 3; x++)
            {
                if (tempList.First().Key == x && tempList.Last().Key == x + 3)
                {
                    return true;
                }
            }
            return false;
        }
        public int CalculateDiceTotal() => scoreContainer.DiceList.Sum(x => x.Value);
    }   
}