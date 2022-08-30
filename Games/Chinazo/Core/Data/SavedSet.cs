namespace Chinazo.Core.Data;
public class SavedSet
{
    public DeckRegularDict<ChinazoCard> CardList { get; set; } = new();
    public EnumRummyType WhatSet { get; set; }
    public bool UseSecond { get; set; }
    public int FirstNumber { get; set; }
}