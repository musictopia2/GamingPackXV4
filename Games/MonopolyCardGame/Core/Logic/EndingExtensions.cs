namespace MonopolyCardGame.Core.Logic;
internal static class EndingExtensions
{
    public static bool HasAnyMonopolyPlayed(this MonopolyCardGameVMData model)
    {
        for (int x = 1; x <= model.TempSets1.HowManySets; x++)
        {
            var list = model.WhatSet(x);
            if (list.Count > 0)
            {
                if (list.CanGoOut(true) == true)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public static bool HasMonopolyInHand(this MonopolyCardGamePlayerItem player)
    {
        var firstList = player.MainHandList.Where(x => x.WhatCard == EnumCardType.IsProperty || x.WhatCard == EnumCardType.IsRailRoad || x.WhatCard == EnumCardType.IsUtilities).ToRegularDeckDict();
        //will not find out if you placed houses.
        var groups = firstList.GroupBy(x => x.WhatCard);
        foreach (var item in groups)
        {
            if (item.Key == EnumCardType.IsRailRoad && item.Count() > 1)
            {
                return true;
            }
            if (item.Key == EnumCardType.IsUtilities && item.Count() == 2)
            {
                return true;
            }
            var card = item.First();
            if (card.Money == 50 || card.Money == 400)
            {
                if (item.Count() == 2)
                {
                    return true;
                }
            }
            if (item.Count() == 3)
            {
                return true;
            }
        }
        return false;
    }
}