namespace YaBlewIt.Core.Logic;
public static class Extensions
{
    extension (IEnumerable<YaBlewItCardInformation> list)
    {
        public BasicList<int> GetContainedNumbers()
        {
            HashSet<int> output = new();
            foreach (var card in list)
            {
                if (card.FirstNumber > 0)
                {
                    output.Add(card.FirstNumber);
                }
                if (card.SecondNumber > 0)
                {
                    output.Add(card.SecondNumber);
                }
            }
            return output.ToBasicList();
        }
        public int TotalPoints(YaBlewItPlayerItem player)
        {
            var wilds = list.Where(x => x.CardColor == EnumColors.Wild).ToBasicList(); //will use for the best category possible.
            var filters = list.Where(x => x.CardColor != EnumColors.Wild && x.CardCategory == EnumCardCategory.Gem).ToBasicList();
            var groups = filters.GroupBy(x => x.CardColor).ToBasicList();
            var nexts = GetCountGroup(groups);
            nexts = nexts.OrderByDescending(x => x.Count).ToBasicList();
            int output = 0;
            int sum;
            bool usedWilds = false;
            foreach (var item in nexts)
            {
                int count = item.Count;
                if (item.Color != player.CursedGem && usedWilds == false)
                {
                    usedWilds = true;
                    count += wilds.Count;
                }
                if (count == 1)
                {
                    sum = 1;
                }
                else if (count == 2)
                {
                    sum = 3;
                }
                else if (count == 3)
                {
                    sum = 6;
                }
                else if (count == 4)
                {
                    sum = 10;
                }
                else if (count == 5)
                {
                    sum = 15;
                }
                else if (count >= 6)
                {
                    sum = 20;
                }
                else
                {
                    sum = 0;
                }
                if (item.Color == player.CursedGem)
                {
                    sum *= -1;
                }
                output += sum;
            }
            if (usedWilds == false)
            {
                if (wilds.Count == 1)
                {
                    return 1;
                }
                if (wilds.Count == 2)
                {
                    return 3;
                }
                if (wilds.Count == 3)
                {
                    return 6;
                }
                if (wilds.Count == 4)
                {
                    return 10;
                }
                if (wilds.Count > 4)
                {
                    throw new CustomBasicException("You cannot have more than 4 wilds");
                }
            }
            return output;
        }
    }
    private static int CardCount(IEnumerable<YaBlewItCardInformation> cards)
    {
        int output = 0;
        foreach (var card in cards)
        {
            if (card.SecondNumber > 0)
            {
                output += 2;
            }
            else
            {
                output++;
            }
        }
        return output;
    }
    private static BasicList<CountClass> GetCountGroup(BasicList<IGrouping<EnumColors, YaBlewItCardInformation>> groups)
    {
        BasicList<CountClass> output = new();
        foreach (var group in groups)
        {
            int count = CardCount(group);
            CountClass temp = new(count, group.Key, group);
            output.Add(temp);
        }
        return output;
    }   
}