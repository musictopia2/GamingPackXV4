﻿namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;

public abstract class RegularDeckOfCardsGameClass<R> where R : IRegularCard, new() //needs to be this way still because the cribbage one needs the rummy ones.
{
    public GenericCardShuffler<R> DeckList;
    private bool _opened;
    protected DeckObservablePile<R>? DeckPile;
    public virtual async Task NewGameAsync(DeckObservablePile<R> deck)
    {
        DeckPile = deck;
        if (_opened == false && await CanOpenSavedSinglePlayerGameAsync())
        {
            _opened = true;
            await OpenSavedGameAsync();
            return;
        }
        ShuffleCards();
    }
    protected abstract void AfterShuffle();
    private void ShuffleCards() // the basics should not worry about how to handle the clicking.  besides, since its shared, something else can call it and do what it wants.
    {
        DeckList.ShuffleObjects();
        DeckPile!.OriginalList(DeckList);
        AfterShuffle();
    }
    public abstract Task<bool> CanOpenSavedSinglePlayerGameAsync();
    public abstract Task OpenSavedGameAsync(); //since you have to do the entire part of the autoresume, then no need for after loading saved game.
    protected virtual void AfterLoadingBasicControls() { }
    public RegularDeckOfCardsGameClass()
    {
        DeckList = new();
    }
}