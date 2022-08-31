namespace Phase10.Core.SetClasses;
public class SavedSet
{
    public DeckRegularDict<Phase10CardInformation> CardList { get; set; } = new();
    public EnumPhase10Sets WhatSet { get; set; }
}