namespace MonopolyCardGame.Core.Logic;
internal static class EndingExtensions
{
    extension(MonopolyCardGameVMData model)
    {
        public bool HasAnyMonopolyPlayed
        {
            get
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
        }
    }
    extension(MonopolyCardGamePlayerItem player)
    {
        public bool HasMonopolyInHand
        {
            get
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
                    var other = item.GroupBy(x => x.Money);
                    foreach (var lasts in other)
                    {
                        //var card = lasts.First();
                        int count = lasts.Count();
                        if (lasts.Key == 50 || lasts.Key == 400)
                        {
                            if (count == 2)
                            {
                                return true;
                            }

                        }
                        else if (count == 3)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

        }
    }
}