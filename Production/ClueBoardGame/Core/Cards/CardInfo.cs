namespace ClueBoardGame.Core.Cards;
public class CardInfo : MainInfo
{
    public CardInfo()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public EnumCardType WhatType { get; set; }
    public EnumCardValues CardValue { get; set; }
    public int Number { get; set; }
    public override void Populate(int chosen)
    {
        throw new CustomBasicException("You have to use the global function so we can get the name.");
    }
    public override void Reset() { }
}