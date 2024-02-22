using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyCardGame.Core.Logic;
internal static class EndingExtensions
{
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