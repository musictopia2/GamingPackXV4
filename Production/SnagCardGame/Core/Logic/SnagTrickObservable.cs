namespace SnagCardGame.Core.Logic;
[SingletonGame]
public class SnagTrickObservable(SnagCardGameGameContainer gameContainer) : BasicTrickAreaObservable<EnumSuitList, SnagCardGameCardInformation>(gameContainer.Command, gameContainer.Aggregator),
    ITrickPlay, IAdvancedTrickProcesses, IMultiplayerTrick<EnumSuitList, SnagCardGameCardInformation, SnagCardGamePlayerItem>
{
    public BasicList<TrickCoordinate>? ViewList { get; set; }
    public void SelectCard(int deck)
    {
        SnagCardGameCardInformation thisCard = CardList.GetSpecificItem(deck);
        if (thisCard.IsSelected == true || thisCard.IsUnknown == true || thisCard.Visible == false)
        {
            throw new CustomBasicException("Card already selecte or shows as unknown.  Rethink");
        }
        thisCard.IsSelected = true;
    }
    public BasicList<SnagCardGameCardInformation> ListCardsLeft()
    {
        var tempList = CardList.Where(items => items.Visible == true && items.IsUnknown == false).ToRegularDeckDict();
        if (tempList.Count == 4)
        {
            throw new CustomBasicException("There can't be 4 cards left over");
        }
        return tempList;
    }
    public int HowManyCardsLeft => CardList.Count(items => items.Visible == true && items.IsUnknown == false);
    public int GetLastCard => CardList.Single(items => items.Visible == true && items.IsUnknown == false).Deck;
    public void RemoveCard(int deck)
    {
        var thisCard = CardList.GetSpecificItem(deck);
        if (thisCard.Visible == false || thisCard.IsUnknown == true)
        {
            throw new CustomBasicException("Cannot remove card because it was already unknown or not visible.  Rethink");
        }
        thisCard.Visible = false;
    }
    private int GetCardIndex()
    {
        var thisC = (from items in ViewList
                     where items.Player == gameContainer.WhoTurn && items.PossibleDummy == !gameContainer.SaveRoot!.FirstCardPlayed
                     select items).Single(); // maybe its opposite (?)
        return ViewList!.IndexOf(thisC); // hopefully this simple (?)
    }
    private BasicList<TrickCoordinate> GetCoordinateList()
    {
        if (gameContainer.PlayerList!.Count != 2)
        {
            throw new CustomBasicException("This is a 2 player game.");
        }
        BasicList<TrickCoordinate> output = [];
        int y = gameContainer.SelfPlayer;
        int x;
        TrickCoordinate thisPlayer;
        for (x = 1; x <= 2; x++)
        {
            thisPlayer = new TrickCoordinate();
            if (x == 1)
            {
                thisPlayer.IsSelf = true;
                thisPlayer.Column = 2;
                thisPlayer.Text = "Your Hand:";
            }
            else
            {
                thisPlayer.PossibleDummy = true;
                thisPlayer.Column = 1;
                thisPlayer.Text = "Your Bar:";
            }
            thisPlayer.Row = 2;
            thisPlayer.Player = y;
            output.Add(thisPlayer);
        }
        if (y == 1)
        {
            y = 2;
        }
        else
        {
            y = 1;
        }
        for (x = 1; x <= 2; x++)
        {
            thisPlayer = new ();
            thisPlayer.Row = 1;
            thisPlayer.Player = y;
            if (x == 1)
            {
                thisPlayer.Column = 2;
                thisPlayer.Text = "Opponent Hand:";
            }
            else
            {
                thisPlayer.PossibleDummy = true;
                thisPlayer.Text = "Opponent Bar:";
                thisPlayer.Column = 1;
            }
            output.Add(thisPlayer);
        }
        return output;
    }
    protected override async Task ProcessCardClickAsync(SnagCardGameCardInformation thisCard)
    {
        if (gameContainer.TakeCardAsync == null)
        {
            throw new CustomBasicException("Nobody is handling takecardasync.  Rethink");
        }
        if (gameContainer.SaveRoot!.GameStatus == EnumStatusList.ChooseCards)
        {
            if (thisCard.Visible == false || thisCard.IsUnknown == true)
            {
                return;
            }
            await gameContainer.TakeCardAsync(thisCard.Deck);
            return;
        }
        int index = CardList.IndexOf(thisCard);
        if (index > 1)
        {
            return;
        }
        if (gameContainer.SaveRoot.FirstCardPlayed == true && index == 0)
        {
            await gameContainer.CardClickedAsync!.Invoke();
            return;
        }
        if (gameContainer.SaveRoot.FirstCardPlayed == false && index == 1)
        {
            await gameContainer.CardClickedAsync!.Invoke();
            return;
        }
    }
    public void ClearBoard()
    {
        DeckRegularDict<SnagCardGameCardInformation> tempList = [];
        for (int x = 1; x <= 4; x++)
        {
            SnagCardGameCardInformation thisCard = new();
            thisCard.Populate(x);
            thisCard.Deck += 1000; //this was the workaround.
            thisCard.IsUnknown = true;
            thisCard.Visible = true;
            tempList.Add(thisCard);
        }
        gameContainer.SaveRoot!.TrickList.Clear();
        CardList.ReplaceRange(tempList);
        Visible = true;
    }
    public void LoadGame()
    {
        var tempList = gameContainer.SaveRoot!.TrickList.ToRegularDeckDict();
        ClearBoard();
        if (tempList.Count == 0)
        {
            return;
        }
        int index;
        int tempTurn;
        SnagCardGameCardInformation lastCard;
        tempTurn = gameContainer.WhoTurn;
        DeckRegularDict<SnagCardGameCardInformation> otherList = [];
        int x = 0;
        tempList.ForEach(thisCard =>
        {
            if (thisCard.Player == 0)
            {
                throw new CustomBasicException("The Player Cannot Be 0");
            }
            gameContainer.WhoTurn = thisCard.Player;
            gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
            x++;
            if (x == 1)
            {
                gameContainer.SaveRoot.FirstCardPlayed = false;
            }
            else
            {
                gameContainer.SaveRoot.FirstCardPlayed = true;
            }
            index = GetCardIndex();
            lastCard = gameContainer.GetBrandNewCard(thisCard.Deck);
            lastCard.Player = thisCard.Player;
            lastCard.IsUnknown = thisCard.IsUnknown;
            lastCard.Visible = thisCard.Visible;
            TradeCard(index, lastCard);
            otherList.Add(lastCard);
        });
        gameContainer.SaveRoot.TrickList.ReplaceRange(otherList);
        gameContainer.SaveRoot.FirstCardPlayed = true;
        gameContainer.WhoTurn = tempTurn;
    }
    public void FirstLoad()
    {
        if (gameContainer.PlayerList!.Count != 2)
        {
            throw new CustomBasicException("Must have 2 players only.");
        }
        ViewList = GetCoordinateList();
        if (ViewList.First().IsSelf == false)
        {
            throw new CustomBasicException("First must be self");
        }
        int x;
        MaxPlayers = 4;
        CardList.Clear();
        for (x = 1; x <= 4; x++)
        {
            SnagCardGameCardInformation newCard = new();
            newCard.Populate(x);
            newCard.IsUnknown = true;
            newCard.Deck = x + 1000; //try this way.  to at least make it work.
            CardList.Add(newCard);
        }
    }
    public async Task PlayCardAsync(int deck)
    {
        SnagCardGameCardInformation thisCard = gameContainer.GetSpecificCardFromDeck(deck);
        thisCard.Player = gameContainer.WhoTurn;
        int index;
        index = GetCardIndex();
        if (index == -1)
        {
            throw new CustomBasicException("Index cannot be -1");
        }
        SnagCardGameCardInformation newCard;
        newCard = gameContainer.GetBrandNewCard(deck);
        newCard.Player = gameContainer.WhoTurn;
        newCard.Visible = true;
        gameContainer.SaveRoot!.TrickList.Add(newCard);
        TradeCard(index, newCard);
        if (gameContainer.SaveRoot.TrickList.Count == 3)
        {
            await gameContainer.EndTrickAsync!.Invoke();
        }
        else
        {
            await gameContainer.ContinueTrickAsync!.Invoke();
        }
    }
    public Task AnimateWinAsync(int wins)
    {
        return Task.CompletedTask;
    }
    public SnagCardGamePlayerItem GetSpecificPlayer(int id)
    {
        return gameContainer.PlayerList![id];
    }
}
