
namespace ClockSolitaire.Core.Logic;
public class ClockBoard : ClockObservable
{
    public ClockBoard(IClockVM thisMod,
        ClockSolitaireMainGameClass mainGame,
        CommandContainer command,
        IEventAggregator aggregator
        ) : base(thisMod, command, aggregator)
    {
        _mainGame = mainGame;
        _aggregator = aggregator;
        ShowCenter = true;
        LoadBoard();
    }
    public int CardsLeft
    {
        get => _mainGame.SaveRoot.CardsLeft;
        private set => _mainGame.SaveRoot.CardsLeft = value;
    }
    private int PreviousOne
    {
        get => _mainGame.SaveRoot.PreviousOne;
        set => _mainGame.SaveRoot.PreviousOne = value;
    }
    private bool _wasSaved;
    private readonly ClockSolitaireMainGameClass _mainGame;
    private readonly IEventAggregator _aggregator;

    public override void LoadSavedClocks(BasicList<ClockInfo> thisList)
    {
        _wasSaved = true;
        if (_mainGame.SaveRoot.CurrentCard == 0)
        {
            CurrentCard = null;
        }
        else
        {
            CurrentCard = new SolitaireCard();
            CurrentCard.Populate(_mainGame.SaveRoot.CurrentCard); //to clone.
        }
        CurrentClock = _mainGame.SaveRoot.PreviousOne + 1;
        base.LoadSavedClocks(thisList);
        SendMessage(PreviousOne, EnumCardMessageCategory.Known);
    }
    public void SendSavedMessage()
    {
        if (_wasSaved == false)
        {
            SendMessage(12, EnumCardMessageCategory.Known);
            return;
        }
        SendMessage(PreviousOne, EnumCardMessageCategory.Known);
    }
    public void SaveGame()
    {
        if (CurrentCard == null)
        {
            _mainGame.SaveRoot.CurrentCard = 0;
        }
        else
        {
            _mainGame.SaveRoot.CurrentCard = CurrentCard.Deck;
        }
        _mainGame.SaveRoot.SavedClocks = ClockList!.ToBasicList();
    }
    public void NewGame(DeckRegularDict<SolitaireCard> thisCol)
    {
        CardsLeft = 51; //because one is given.
        if (thisCol.Count != 52)
        {
            throw new CustomBasicException("Must have 52 cards");
        }
        ClearBoard();
        int y = 0;
        4.Times(x =>
        {
            ClockList!.ForEach(thisClock =>
            {
                y++;
                var thisCard = thisCol[y - 1]; //because 0 based
                if (y == 52)
                {
                    CurrentCard = thisCard;
                    CurrentCard.IsUnknown = false;
                }
                else
                {
                    thisClock.CardList.Add(thisCard);
                }
            });
        });
        EnablePiles();
        PreviousOne = 12;
        CurrentClock = 13;
        SendMessage(12, EnumCardMessageCategory.Known);
    }
    public bool HasWon() => CardsLeft == 0;
    public bool IsGameOver()
    {
        if (CurrentCard!.Value != EnumRegularCardValueList.King)
        {
            return false;
        }
        return HasCard(12) == false;
    }
    public bool IsValidMove(int whichOne) => CurrentCard!.Value.Value == whichOne + 1;
    public void MakeMove(int whichOne)
    {
        SendMessage(PreviousOne, EnumCardMessageCategory.Hidden);
        PreviousOne = whichOne;
        CardsLeft--;
        CurrentCard = GetLastCard(whichOne);
        RemoveCardFromPile(whichOne);
        SendMessage(whichOne, EnumCardMessageCategory.Known);
    }
    protected override Task ClickCurrentCardProcessAsync()
    {
        return OnClockClickedAsync(PreviousOne);
    }
    private void SendMessage(int index, EnumCardMessageCategory category)
    {
        CurrentCardEventModel thisMessage = new();
        thisMessage.ThisClock = ClockList![index];
        thisMessage.ThisCategory = category;
        _aggregator.PublishAll(thisMessage);
    }
}