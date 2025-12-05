namespace MonopolyCardGame.Core.Logic;
internal static class GoOutExtensions
{
    extension (IDeckDict<MonopolyCardGameCardInformation> group)
    {
        public bool CanGoOut(bool onlyOne = false)
        {
            //SingleInfo = PlayerList!.GetWhoPlayer();
            var tempCol = group.ToRegularDeckDict();
            var groupList = tempCol.GroupBy(items => items.Group).ToBasicList();
            bool hasRailRoad = group.Any(items => items.WhatCard == EnumCardType.IsRailRoad);
            bool hasUtilities = group.Any(items => items.WhatCard == EnumCardType.IsUtilities);
            int numWilds = group.Count(items => items.WhatCard == EnumCardType.IsChance);
            if (hasRailRoad && hasUtilities && onlyOne)
            {
                return false; //this means you cannot have both railroads and utilties.
            }
            if (numWilds < 2 && hasRailRoad == false && hasUtilities == false && groupList.Count == 0)
            {
                return false; //cannot go out because do not have any properties, no railroads, no utilities, and not enough wilds
            }
            var temps = tempCol.Where(items => (int)items.WhatCard > 3 && (int)items.WhatCard < 7).ToRegularDeckDict();
            tempCol.RemoveGivenList(temps);
            BasicList<int> setList = [];
            DeckRegularDict<MonopolyCardGameCardInformation> monCol;
            foreach (var thisGroup in groupList)
            {
                if (thisGroup.Key > 0)
                {
                    monCol = MonopolyCardGameMainGameClass.MonopolyCol(tempCol, thisGroup.Key, EnumCardType.IsProperty);
                    if (monCol.Count == 0)
                    {
                        return false;
                    }
                    setList.Add(thisGroup.Key);
                }
            }
            if (hasRailRoad && setList.Count > 0 && onlyOne)
            {
                return false;
            }
            if (hasUtilities && setList.Count > 0 && onlyOne)
            {
                return false;
            }
            if (setList.Count > 1 && onlyOne)
            {
                return false;
            }
            if (hasRailRoad)
            {
                monCol = MonopolyCardGameMainGameClass.MonopolyCol(tempCol, 0, EnumCardType.IsRailRoad);
            }
            if (hasUtilities)
            {
                monCol = MonopolyCardGameMainGameClass.MonopolyCol(tempCol, 0, EnumCardType.IsUtilities);
            }
            setList.ForEach(thisSet =>
            {
                MonopolyCardGameMainGameClass.HouseCollection(tempCol); //to filter further
            });
            tempCol.RemoveAllOnly(items => items.WhatCard == EnumCardType.IsChance);
            return tempCol.Count == 0;
        }
    }
    extension (MonopolyCardGameVMData model)
    {
        public bool HasAllValidMonopolies
        {
            get
            {
                for (int x = 1; x <= model!.TempSets1.HowManySets; x++)
                {
                    var list = model.WhatSet(x);
                    if (list.Count > 0)
                    {
                        if (list.CanGoOut(true) == false)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }   
        }
    }
}