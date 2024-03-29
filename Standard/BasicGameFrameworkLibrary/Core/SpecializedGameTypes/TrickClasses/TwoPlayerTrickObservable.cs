﻿namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;
public class TwoPlayerTrickObservable<SU, T, P, SA>(TrickGameContainer<T, P, SA, SU> gameContainer) : BasicTrickAreaObservable<SU, T>(gameContainer.Command, gameContainer.Aggregator),
    ITrickPlay, IAdvancedTrickProcesses
    where SU : IFastEnumSimple
    where T : class, ITrickCard<SU>, new()
    where P : class, IPlayerTrick<SU, T>, new()
    where SA : BasicSavedTrickGamesClass<SU, T, P>, new()
{
    public void FirstLoad()
    {
        if (gameContainer.PlayerList!.Count != 2)
        {
            throw new CustomBasicException("Must have 2 players in order to load");
        }
        int x;
        CardList.Clear();
        for (x = 1; x <= 2; x++)
        {
            T newCard = new();
            newCard.Populate(x);
            newCard.Deck = x + 1000;
            newCard.IsUnknown = true;
            CardList.Add(newCard);
        }
    }
    protected int GetCardIndex()
    {
        if (gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            return 0;
        }
        return 1;
    }
    public bool IsLead => OrderList.Count == 0;
    public void ClearBoard()
    {
        DeckRegularDict<T> tempList = new();
        int x = 0;
        foreach (var thisCard in CardList)
        {
            T tempCard = new();
            x++;
            tempCard.Populate(x);
            tempCard.Deck += 1000;
            tempCard.IsUnknown = true;
            tempCard.Visible = true;
            tempList.Add(tempCard);
        }
        OrderList.Clear();
        CardList.ReplaceRange(tempList);
        Visible = true;
    }
    public virtual void LoadGame()
    {
        var tempList = OrderList.ToRegularDeckDict();
        ClearBoard();
        if (tempList.Count == 0)
        {
            return;
        }
        int index;
        int tempTurn;
        T lastCard;
        tempTurn = gameContainer.WhoTurn;
        DeckRegularDict<T> otherList = new();
        tempList.ForEach(thisCard =>
        {
            if (thisCard.Player == 0)
            {
                throw new CustomBasicException("The Player Cannot Be 0");
            }
            gameContainer.WhoTurn = thisCard.Player;
            gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
            index = GetCardIndex();
            lastCard = gameContainer.GetBrandNewCard(thisCard.Deck);
            lastCard.Player = thisCard.Player;
            TradeCard(index, lastCard);
            otherList.Add(lastCard);
        });
        OrderList.ReplaceRange(otherList);
        gameContainer.WhoTurn = tempTurn;
    }
    protected T GetWinningCard(int wins)
    {
        return OrderList.Single(items => items.Player == wins);
    }
    public async Task AnimateWinAsync(int wins)
    {
        var thisCard = GetWinningCard(wins);
        WinCard = thisCard;
        await AnimateWinAsync();
    }
    public async Task PlayCardAsync(int deck)
    {
        T thisCard = gameContainer.GetSpecificCardFromDeck(deck);
        thisCard.Player = gameContainer.WhoTurn;
        int index = GetCardIndex();
        T newCard = gameContainer.GetBrandNewCard(deck);
        newCard.Player = gameContainer.WhoTurn;
        newCard.Visible = true;
        OrderList.Add(newCard);
        TradeCard(index, newCard);
        gameContainer.Command.UpdateAll();
        await AfterPlayCardAsync(thisCard);
    }
    protected virtual async Task AfterPlayCardAsync(T thisCard) 
    {
        if (OrderList.Count == gameContainer.PlayerList!.Count)
        {
            await gameContainer.EndTrickAsync!.Invoke();
        }
        else
        {
            await gameContainer.ContinueTrickAsync!.Invoke();
        }
    }
    protected DeckRegularDict<T> OrderList => gameContainer.SaveRoot!.TrickList;
    protected override async Task ProcessCardClickAsync(T thisCard)
    {
        if (CardList.IndexOf(thisCard) == 0)
        {
            await gameContainer.CardClickedAsync!.Invoke();
        }
    }
}