namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;
public class SeveralPlayersTrickObservable<SU, T, P, SA>(TrickGameContainer<T, P, SA, SU> gameContainer) : BasicTrickAreaObservable<SU, T>(gameContainer.Command, gameContainer.Aggregator), IMultiplayerTrick<SU, T, P>
    , ITrickPlay, IAdvancedTrickProcesses
    where SU : IFastEnumSimple
    where T : class, ITrickCard<SU>, new()
    where P : class, IPlayerTrick<SU, T>, new()
    where SA : BasicSavedTrickGamesClass<SU, T, P>, new()
{
    public BasicList<TrickCoordinate>? ViewList { get; set; }
    public DeckRegularDict<T> OrderList => gameContainer.SaveRoot!.TrickList;
    private BasicList<TrickCoordinate> GetCoordinateList()
    {
        BasicList<TrickCoordinate> output = new();
        int howManyPlayers = gameContainer.PlayerList!.Count;
        TrickCoordinate thisPlayer;
        if (howManyPlayers == 2)
        {
            thisPlayer = new();
            thisPlayer.IsSelf = true;
            thisPlayer.Row = 1;
            thisPlayer.Column = 1;
            thisPlayer.Player = gameContainer.SelfPlayer;
            output.Add(thisPlayer);
            thisPlayer = new();
            thisPlayer.Column = 2;
            thisPlayer.Row = 1;
            if (gameContainer.SelfPlayer == 1)
            {
                thisPlayer.Player = 2;
            }
            else
            {
                thisPlayer.Player = 1;
            }
            output.Add(thisPlayer);
            return output;
        }
        int howManyBottom;
        int howManyTop;
        if (howManyPlayers <= 4)
        {
            howManyBottom = 2;
        }
        else if (howManyPlayers <= 6)
        {
            howManyBottom = 3;
        }
        else
        {
            howManyBottom = 4;
        }
        howManyTop = howManyPlayers - howManyBottom;
        if (howManyTop > howManyBottom)
        {
            throw new Exception("Top can never have more than bottom");
        }
        int x;
        int y;
        y = gameContainer.SelfPlayer;
        var loopTo = howManyBottom;
        for (x = 1; x <= loopTo; x++)
        {
            thisPlayer = new TrickCoordinate();
            if (x == 1)
            {
                thisPlayer.IsSelf = true;
            }
            thisPlayer.Row = 2;
            thisPlayer.Column = x;
            thisPlayer.Player = y;
            y += 1;
            if (y > howManyPlayers)
            {
                y = 1;
            }
            output.Add(thisPlayer);
        }
        var loopTo1 = howManyTop;
        for (x = 1; x <= loopTo1; x++)
        {
            thisPlayer = new();
            thisPlayer.Column = x;
            thisPlayer.Row = 1;
            thisPlayer.Player = y;
            y += 1;
            if (y > howManyPlayers)
            {
                y = 1;
            }
            output.Add(thisPlayer);
        }
        return output;
    }
    public void FirstLoad()
    {
        if (gameContainer.PlayerList!.Count == 0)
        {
            throw new CustomBasicException("Playerlist Has Not Been Initialized Yet");
        }
        ViewList = GetCoordinateList();
        if (ViewList.First().IsSelf == false)
        {
            throw new CustomBasicException("First must be self");
        }
        int x;
        MaxPlayers = gameContainer.PlayerList.Count;
        CardList.Clear();
        for (x = 1; x <= MaxPlayers; x++)
        {
            T newCard = new();
            newCard.Populate(x);
            newCard.IsUnknown = true;
            newCard.Deck = x + 1000;
            CardList.Add(newCard);
        }
    }
    public virtual void ClearBoard()
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
    private int GetCardIndex()
    {
        var thisC = (from xx in ViewList
                     where xx.Player == gameContainer.WhoTurn
                     select xx).Single();
        return ViewList!.IndexOf(thisC);
    }
    protected virtual void PopulateNewCard(T oldCard, ref T newCard) { }
    public virtual void LoadGame()
    {
        var tempList = OrderList.ToRegularDeckDict();
        ClearBoard();
        ViewList = GetCoordinateList();
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
    protected override async Task ProcessCardClickAsync(T thisCard)
    {
        if (CardList.IndexOf(thisCard) == 0)
        {
            await gameContainer.CardClickedAsync!.Invoke();
        }
    }
    public P GetSpecificPlayer(int id)
    {
        return gameContainer.PlayerList![id];
    }
}