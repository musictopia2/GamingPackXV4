﻿namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;
public abstract class PossibleDummyTrickObservable<SU, T, P, SA>(TrickGameContainer<T, P, SA, SU> gameContainer) : BasicTrickAreaObservable<SU, T>(gameContainer.Command, gameContainer.Aggregator), IMultiplayerTrick<SU, T, P>
    , ITrickPlay
    where SU : IFastEnumSimple
    where T : class, ITrickCard<SU>, new()
    where P : class, IPlayerTrick<SU, T>, new()
    where SA : BasicSavedTrickGamesClass<SU, T, P>, new()
{
    protected abstract bool UseDummy { get; set; }
    public BasicList<TrickCoordinate>? ViewList { get; set; }
    protected abstract int GetCardIndex(); // this is different too.
    protected DeckRegularDict<T> OrderList => gameContainer.SaveRoot!.TrickList;
    protected abstract void PopulateNewCard(T oldCard, ref T newCard);
    protected abstract void PopulateOldCard(T oldCard);
    protected virtual int GetMaxCount()
    {
        if (UseDummy == true)
        {
            return 3;
        }
        return 4;
    }
    public async Task PlayCardAsync(int deck)
    {
        T thisCard = gameContainer.GetSpecificCardFromDeck(deck);
        thisCard.Player = gameContainer.WhoTurn;
        PopulateOldCard(thisCard);
        int index;
        index = GetCardIndex();
        if (index == -1)
        {
            throw new CustomBasicException("Index cannot be -1");
        }
        T newCard;
        newCard = gameContainer.GetBrandNewCard(deck);
        newCard.Player = gameContainer.WhoTurn;
        newCard.Visible = true;
        PopulateOldCard(newCard);
        OrderList.Add(newCard);
        TradeCard(index, newCard);
        int nums;
        nums = GetMaxCount();
        gameContainer.Command.UpdateAll();
        if (OrderList.Count == nums)
        {
            await gameContainer.EndTrickAsync!.Invoke();
        }
        else
        {
            await gameContainer.ContinueTrickAsync!.Invoke();
        }
    }
    protected virtual string FirstHumanText()
    {
        return "Yours";
    }
    protected virtual string FirstOpponentText()
    {
        return "Opponents";
    }
    protected virtual string DummyHumanText()
    {
        return "Dummy";
    }
    protected virtual string DummyOpponentText()
    {
        return "Dummy";
    }
    protected BasicList<TrickCoordinate> GetCoordinateList()
    {
        BasicList<TrickCoordinate> output = new();
        int howManyPlayers = gameContainer.PlayerList!.Count;
        TrickCoordinate thisPlayer;
        if (howManyPlayers == 2)
        {
            if (UseDummy == false)
            {
                throw new CustomBasicException("Must use dummy if 2 players.  If no dummy is used, try using TwoPlayerTrickViewModel");
            }
            thisPlayer = new();
            thisPlayer.IsSelf = true;
            thisPlayer.Row = 2;
            thisPlayer.Column = 1;
            thisPlayer.Player = gameContainer.SelfPlayer;
            thisPlayer.Text = FirstHumanText();
            output.Add(thisPlayer);
            thisPlayer = new();
            thisPlayer.Column = 1;
            thisPlayer.Row = 1;
            thisPlayer.Text = FirstOpponentText();
            if (gameContainer.SelfPlayer == 1)
            {
                thisPlayer.Player = 2;
            }
            else
            {
                thisPlayer.Player = 1;
            }
            int oldPlayer;
            oldPlayer = thisPlayer.Player;
            output.Add(thisPlayer);
            thisPlayer = new();
            thisPlayer.Player = oldPlayer;
            thisPlayer.PossibleDummy = true;
            thisPlayer.Column = 2;
            thisPlayer.Row = 1;
            thisPlayer.Text = DummyOpponentText();
            output.Add(thisPlayer);
            thisPlayer = new();
            thisPlayer.Player = gameContainer.SelfPlayer;
            thisPlayer.PossibleDummy = true;
            thisPlayer.Column = 2;
            thisPlayer.Row = 2;
            thisPlayer.Text = DummyHumanText();
            output.Add(thisPlayer);
            return output;
        }
        

        //attempt to support 3 players.  well see what happens (refer to the severaltricks since that had no issues

        //if (howManyPlayers == 3)
        //{
        //    throw new CustomBasicException("Currently, can't support 3 players because even the old version was too buggy.  Therefore, only 2 players are supported for now");
        //}
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
    protected virtual void BeforeFirstLoad() { }
    public void FirstLoad()
    {
        if (gameContainer.PlayerList!.Count == 0)
        {
            throw new CustomBasicException("Playerlist Has Not Been Initialized Yet");
        }
        BeforeFirstLoad();
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
            T newCard = new();
            newCard.Populate(x);
            newCard.IsUnknown = true;
            newCard.Deck = x + 1000;
            CardList.Add(newCard);
        }
    }
    public P GetSpecificPlayer(int id)
    {
        return gameContainer.PlayerList![id];
    }
}