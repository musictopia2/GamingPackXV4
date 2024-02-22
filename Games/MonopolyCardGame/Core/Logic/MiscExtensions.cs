namespace MonopolyCardGame.Core.Logic;
internal static class MiscExtensions
{
    public static DeckRegularDict<MonopolyCardGameCardInformation> WhatSet(this MonopolyCardGameVMData model, int whichOne)
    {
        return model.TempSets1!.ObjectList(whichOne);
    }
}