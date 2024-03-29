﻿namespace Rook.Core.Logic;
[SingletonGame]
[AutoReset]
public class RookTrickAreaCP : PossibleDummyTrickObservable<EnumColorTypes, RookCardInformation, RookPlayerItem, RookSaveInfo>, IAdvancedTrickProcesses
{
    private readonly RookGameContainer _gameContainer;
    private readonly RookDelegates _delegates;

    public RookTrickAreaCP(RookGameContainer gameContainer, RookDelegates delegates) : base(gameContainer)
    {
        _gameContainer = gameContainer;
        _delegates = delegates;
    }
    protected override bool UseDummy { get; set; }
    protected override int GetCardIndex()
    {
        var thisView = ViewList!.Single(items => items.Player == _gameContainer.WhoTurn && items.PossibleDummy == _gameContainer.SaveRoot!.DummyPlay);
        return ViewList!.IndexOf(thisView);
    }
    protected override void PopulateNewCard(RookCardInformation oldCard, ref RookCardInformation newCard)
    {
        if (_delegates.IsDummy!() == false)
        {
            return;
        }
        newCard.IsDummy = oldCard.IsDummy;
    }
    protected override void PopulateOldCard(RookCardInformation oldCard)
    {
        if (_delegates.IsDummy!() == false)
        {
            return;
        }
        oldCard.IsDummy = _gameContainer.SaveRoot!.DummyPlay;
    }
    protected override async Task ProcessCardClickAsync(RookCardInformation thisCard)
    {
        int index = CardList.IndexOf(thisCard);
        if (index == 1 || index == 2)
        {
            return;
        }
        if (_gameContainer.SaveRoot!.DummyPlay && index == 0)
        {
            await DummyClickAsync();
            return;
        }
        if (index == 3 && _gameContainer.SaveRoot.DummyPlay == false)
        {
            await DummyClickAsync();
            return;
        }
        await _gameContainer.CardClickedAsync!.Invoke();
    }
    private async Task DummyClickAsync()
    {
        if (_gameContainer.PlayerList!.Count == 3)
        {
            return; //because somebody else is playing it.
        }
        await _gameContainer.CardClickedAsync!.Invoke();
    }
    private RookCardInformation GetWinningCard(int wins, bool isDummy)
    {
        return (from items in OrderList
                where items.Player == wins && items.IsDummy == isDummy
                select items).Single();
    }
    public async Task AnimateWinAsync(int wins, bool isDummy)
    {
        var thisCard = GetWinningCard(wins, isDummy);
        _gameContainer.SaveRoot!.DummyPlay = isDummy;
        WinCard = thisCard;
        await AnimateWinAsync();
    }
    protected override void BeforeFirstLoad()
    {
        UseDummy = _gameContainer.PlayerList!.Count == 2;
    }
    protected override int GetMaxCount()
    {
        if (_gameContainer.PlayerList!.Count == 2)
        {
            return base.GetMaxCount();
        }
        return _gameContainer.PlayerList.Count; //i think
    }
    public void ClearBoard()
    {
        DeckRegularDict<RookCardInformation> tempList = new();
        int x;
        int self = _gameContainer.SelfPlayer;
        for (x = 1; x <= 4; x++)
        {
            RookCardInformation thisCard = new();
            thisCard.Populate(x);
            thisCard.Deck += 1000; //this was the workaround.
            thisCard.IsUnknown = true;
            if (x <= 2)
            {
                thisCard.Visible = true;
            }
            else if (x == 3 && self == _gameContainer.SaveRoot!.WonSoFar)
            {
                thisCard.Visible = true;
            }
            else if (x == 4 && self != _gameContainer.SaveRoot!.WonSoFar)
            {
                thisCard.Visible = true;
            }
            else
            {
                thisCard.Visible = false;
            }
            tempList.Add(thisCard);
            if (_gameContainer.PlayerList!.Count ==3 && x== 3)
            {
                break; //break out so it matches.
            }
        }
        OrderList.Clear();
        CardList.ReplaceRange(tempList);
        Visible = true;
    }
    public void NewRound()
    {
        if (_gameContainer.PlayerList!.Count == 2)
        {
            int self = _gameContainer.SelfPlayer;
            if (self == _gameContainer.SaveRoot!.WonSoFar)
            {
                ViewList![2].Visible = true;
                ViewList[3].Visible = false;
            }
            else
            {
                ViewList![2].Visible = false;
                ViewList[3].Visible = true;
            }
        }
        ClearBoard();
    }
    public void LoadGame()
    {
        ViewList = GetCoordinateList();
        if (_gameContainer.SaveRoot!.TrickList.Count == 0)
        {
            NewRound();
            return;
        }
        var tempList = OrderList.ToRegularDeckDict();
        ClearBoard();
        if (tempList.Count == 0)
        {
            throw new CustomBasicException("Rethinking may be required.");
        }
        int index;
        int tempTurn;
        RookCardInformation lastCard;
        tempTurn = _gameContainer.WhoTurn;
        DeckRegularDict<RookCardInformation> otherList = new();
        bool tempDummyPlay = _gameContainer.SaveRoot.DummyPlay;
        tempList.ForEach(thisCard =>
        {
            if (thisCard.Player == 0)
            {
                throw new CustomBasicException("The Player Cannot Be 0");
            }
            _gameContainer.WhoTurn = thisCard.Player;
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            _gameContainer.SaveRoot.DummyPlay = thisCard.IsDummy;
            index = GetCardIndex();
            lastCard = _gameContainer.GetBrandNewCard(thisCard.Deck);
            lastCard.Player = thisCard.Player;
            lastCard.IsDummy = thisCard.IsDummy;
            TradeCard(index, lastCard);
            otherList.Add(lastCard);
        });
        OrderList.ReplaceRange(otherList);
        _gameContainer.WhoTurn = tempTurn;
        _gameContainer.SaveRoot.DummyPlay = tempDummyPlay;
    }
    public Task AnimateWinAsync(int wins)
    {
        throw new CustomBasicException("This time, needs to use the one with dummy player.");
    }
}