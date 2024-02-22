namespace MonopolyCardGame.Core.Logic;
internal static class MiscExtensions
{
    public static DeckRegularDict<MonopolyCardGameCardInformation> WhatSet(this MonopolyCardGameVMData model, int whichOne)
    {
        return model.TempSets1!.ObjectList(whichOne);
    }
    public static bool HasChance(this MonopolyCardGamePlayerItem player, MonopolyCardGameVMData model)
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