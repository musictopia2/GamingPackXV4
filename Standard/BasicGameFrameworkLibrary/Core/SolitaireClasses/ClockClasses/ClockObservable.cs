namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.ClockClasses;
public partial class ClockObservable : IPlainObservable
{
    public bool ShowCenter { get; set; }
    public BasicList<ClockInfo>? ClockList;
    private SizeF _individualSize;
    public SolitaireCard? CurrentCard;
    public int CurrentClock; //will be 1 based because we have the number guide.
    public PlainCommand? ClockCommand { get; set; }
    public CommandContainer Command { get; }
    public void ClearBoard()
    {
        int decks = 10000;
        ClockList!.ForEach(thisClock =>
        {
            thisClock.CardList.Clear();
            var thisCard = new SolitaireCard();
            decks++;
            thisCard.Deck = decks; //because of how the dictionary works.
            thisClock.CardList.Add(thisCard);
            if (ShowCenter || ClockList.IndexOf(thisClock) + 1 <= 4)
            {
                thisClock.LeftGuide = 4;
            }
            else
            {
                thisClock.LeftGuide = 3;
            }
        });
    }
    public void AddCardToPile(int pile, SolitaireCard thisCard)
    {
        if (ShowCenter)
        {
            throw new CustomBasicException("Cannot add card to pile because the goal is to get rid of cards");
        }
        var thisClock = ClockList![pile];
        thisClock.LeftGuide--;
        thisClock.CardList.Add(thisCard);
        _aggregator.PublishAll(thisClock); //try tu allow all for this.  has to retest regular clock solitaire.
    }
    public void RemoveCardFromPile(int pile)
    {
        if (ShowCenter == false)
        {
            throw new CustomBasicException("Cannot remove card because the goal is to put cards to pile");
        }
        var thisClock = ClockList![pile];
        thisClock.CardList.RemoveAt(1);
        thisClock.LeftGuide++;
        if (thisClock.CardList.Count == 1)
        {
            thisClock.IsEnabled = false;
        }
        _aggregator.PublishAll(thisClock);
    }
    public bool HasCard(int pile)
    {
        var thisClock = ClockList![pile];
        return thisClock.CardList.Count > 1;
    }
    public void EnablePiles()
    {
        ClockList!.ForEach(thisClock =>
        {
            thisClock.IsEnabled = true;
            if (ShowCenter)
            {
                thisClock.LeftGuide = 5 - thisClock.CardList.Count;
            }
            else
            {
                if (ShowCenter || ClockList.IndexOf(thisClock) + 1 <= 4)
                {
                    thisClock.LeftGuide = 5 - thisClock.CardList.Count;
                }
                else
                {
                    thisClock.LeftGuide = 4 - thisClock.CardList.Count;
                }
            }
        });
    }
    public SolitaireCard GetLastCard(int pile)
    {
        ClockInfo thisClock = ClockList![pile];
        if (thisClock.CardList.Count < 2 && ShowCenter)
        {
            throw new CustomBasicException("There are no cards to get");
        }
        if (ShowCenter == false)
        {
            return thisClock.CardList.Last();
        }
        else
        {
            return thisClock.CardList[1];
        }
    }
    public BasicList<ClockInfo> GetSavedClocks()
    {
        return ClockList!.ToBasicList();
    }
    public virtual void LoadSavedClocks(BasicList<ClockInfo> thisList)
    {
        ClockList = new(thisList);
        SolitaireCard tempCard = new();
        _individualSize = tempCard.DefaultSize;
        PositionClock();
    }
    protected virtual async Task OnClockClickedAsync(int index)
    {
        await ThisMod.ClockClickedAsync(index);
    }
    protected virtual async Task ClickCurrentCardProcessAsync()
    {
        await Task.CompletedTask;
    }
    public void LoadBoard()
    {
        ClockList = new();
        int maxs;
        if (ShowCenter)
        {
            maxs = 13;
        }
        else
        {
            maxs = 12;
        }
        SolitaireCard tempCard = new();
        _individualSize = tempCard.DefaultSize;
        int decks = 10000;
        maxs.Times(x =>
        {
            ClockInfo thisClock = new();
            if (ShowCenter)
            {
                SolitaireCard thisCard = new ();
                thisCard.Deck = decks;
                decks++;
                thisClock.CardList.Add(thisCard);
                thisClock.NumberGuide = x;
            }
            if (ShowCenter || x <= 4)
            {
                thisClock.LeftGuide = 4;
            }
            else
            {
                thisClock.LeftGuide = 3;
            }
            ClockList.Add(thisClock);
        });
        PositionClock();
    }
    private void FirstClockNotify()
    {
        ClockList!.ForEach(clock =>
        {
            _aggregator.Publish(clock);
        });
    }
    private void PositionClock()
    {
        float diffLeft = _individualSize.Width + 5;
        float diffTop = _individualSize.Height - 5;
        float startLeft = diffLeft * 4;
        float startTop = diffTop;
        float realLeft = startLeft;
        float realTop = startTop;
        int x = 0;
        ClockList!.ForEach(thisClock =>
        {
            x++;
            if (x == 9 && realLeft != 0)
            {
                throw new CustomBasicException("9 was supposed to be 0");
            }
            if (x == 12 && realTop != 0)
            {
                throw new CustomBasicException("12 was supposed to be 0");
            }
            thisClock.Location = new PointF(realLeft, realTop);
            if (x == 1 || x == 2)
            {
                realTop += diffTop;
                realLeft += diffLeft;
            }
            else if (x == 3 || x == 4 || x == 5)
            {
                realLeft -= diffLeft;
                realTop += diffTop;
            }
            else if (x == 6 || x == 7 || x == 8)
            {
                realLeft -= diffLeft;
                realTop -= diffTop;
            }
            else if (x == 9 || x == 10 || x == 11)
            {
                realLeft += diffLeft;
                realTop -= diffTop;
                if (x == 11)
                {
                    realTop = 0;
                }
            }
            else if (x == 12)
            {
                realTop = ClockList[2].Location.Y;
            }
        });
    }
    protected IClockVM ThisMod;
    private readonly IEventAggregator _aggregator;
    [Command(EnumCommandCategory.Plain, Name = nameof(ClockCommand), Can = nameof(CanPrivateClick))]
    private async Task PrivateClickAsync(SolitaireCard card)
    {
        if (CurrentCard != null)
        {
            if (CurrentCard.Deck == card.Deck)
            {
                await ClickCurrentCardProcessAsync();
                return;
            }
        }
        foreach (var thisClock in ClockList!)
        {
            if (thisClock.CardList.Last().Equals(card))
            {
                await OnClockClickedAsync(ClockList.IndexOf(thisClock));
                return;
            }
        }
        throw new CustomBasicException("Found no card.  Rethink");
    }
    private bool CanPrivateClick(SolitaireCard card)
    {
        if (Command is null)
        {
            throw new CustomBasicException("No command");//to stop the warnings.
        }
        return card.IsEnabled == true && card.CardType != EnumRegularCardTypeList.Stop;
    }
    public ClockObservable(IClockVM thisMod,
        CommandContainer command,
        IEventAggregator aggregator)
    {
        ThisMod = thisMod;
        Command = command;
        _aggregator = aggregator;
        CreateCommands(Command);
    }
    partial void CreateCommands(CommandContainer command);
}
