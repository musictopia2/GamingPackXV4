namespace MonasteryCardGame.Core.Data;
public class SavedSet
{
    public EnumMonasterySets WhatType { get; set; }
    public DeckRegularDict<MonasteryCardInfo> CardList { get; set; } = new();
}