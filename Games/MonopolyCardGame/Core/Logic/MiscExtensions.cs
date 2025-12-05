namespace MonopolyCardGame.Core.Logic;
internal static class MiscExtensions
{
    extension (MonopolyCardGameVMData model)
    {
        public DeckRegularDict<MonopolyCardGameCardInformation> WhatSet(int whichOne)
        {
            return model.TempSets1!.ObjectList(whichOne);
        }
    }
    extension (MonopolyCardGamePlayerItem player)
    {
        public bool HasChance(MonopolyCardGameVMData model)
        {
            if (player.PlayerCategory != EnumPlayerCategory.Self)
            {
                return player.MainHandList.Any(x => x.WhatCard == EnumCardType.IsChance);
            }
            if (player.MainHandList.Any(x => x.WhatCard == EnumCardType.IsChance))
            {
                return true;
            }
            return model.TempSets1.ListAllObjects().Any(x => x.WhatCard == EnumCardType.IsChance);
        }
    }
}