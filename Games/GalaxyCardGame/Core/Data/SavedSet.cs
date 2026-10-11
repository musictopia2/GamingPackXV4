namespace GalaxyCardGame.Core.Data;
public class SavedSet
{
    public DeckRegularDict<GalaxyCardGameCardInformation> CardList { get; set; } = [];
    public EnumWhatSets WhatSet;
}