namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
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
    private void ShuffleCards()
    {
        DeckList.ShuffleObjects();
        DeckPile!.OriginalList(DeckList);
        AfterShuffle();
    }
    public abstract Task<bool> CanOpenSavedSinglePlayerGameAsync();
    public abstract Task OpenSavedGameAsync();
    protected virtual void AfterLoadingBasicControls() { }
    public RegularDeckOfCardsGameClass()
    {
        DeckList = new();
    }
}