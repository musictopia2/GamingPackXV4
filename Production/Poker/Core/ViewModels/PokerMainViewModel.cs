namespace Poker.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class PokerMainViewModel(CommandContainer commandContainer,

    IEventAggregator aggregator,
    IGamePackageResolver resolver,
        BasicData basicData
        ) : IBasicEnableProcess,
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>,
    IHandle<NewCardsEventModel>
    , IStateChanged
{
    private partial void Subscribe();
    private partial void Unsubscribe();
    private PokerMainGameClass _mainGame = null!;
    public static BasicList<DisplayCard> PokerList => GlobalClass.PokerList;
    public DeckObservablePile<PokerCardInfo> DeckPile { get; set; } = null!;
    public event Action? StateChanged;
    public static DeckRegularDict<PokerCardInfo> GetCardList => PokerList!.Select(items => items.CurrentCard!)!.ToRegularDeckDict()!;
    public static int SpotsToFill
    {
        get
        {
            if (PokerList.Count == 0)
            {
                return 5;
            }
            return PokerList.Count(items => items.WillHold == false);
        }
    }
    private static void PublishNewCards()
    {
        IEventAggregator aggregator = Resolver!.Resolve<IEventAggregator>();
        aggregator.Publish(new NewCardsEventModel());
    }
    public static void PopulateNewCards(IDeckDict<PokerCardInfo> thisList)
    {
        DisplayCard thisDisplay;
        if (PokerList.Count == 0)
        {
            BasicList<DisplayCard> newList = [];
            if (thisList.Count != 5)
            {
                throw new CustomBasicException("Must have 5 cards for the poker hand");
            }
            thisList.ForEach(thisCard =>
            {
                thisDisplay = new();
                thisDisplay.CurrentCard = thisCard;
                newList.Add(thisDisplay);
            });
            PokerList.ReplaceRange(newList);
            PublishNewCards();
            return;
        }
        var tempList = PokerList.Where(items => items.WillHold == false).ToBasicList();
        if (tempList.Count != thisList.Count)
        {
            throw new CustomBasicException("Mismatch for populating new cards");
        }
        int x = 0;
        tempList.ForEach(temps =>
        {
            var thisCard = thisList[x];
            temps.CurrentCard = thisCard;
            x++;
        });
        PublishNewCards();
    }
    [LabelColumn]
    public partial decimal Money { get; set; }
    public int BetAmount { get; set; } = 5;
    [LabelColumn]
    public partial decimal Winnings { get; set; }
    [LabelColumn]
    public partial string HandLabel { get; set; } = "";
    [LabelColumn]
    public partial int Round { get; set; }
    public bool BetPlaced { get; set; }
    public bool IsRoundOver { get; set; }
    public NumberPicker Bet1 = null!; //decided to have the numberpicker here.  because not only for new game but new round.
    public bool CanHoldUnhold
    {
        get
        {
            if (PokerList.Count == 0)
            {
                return false;
            }
            return !IsRoundOver;
        }
    }
    [Command(EnumCommandCategory.Plain)]
    public void HoldUnhold(DisplayCard display)
    {
        if (display == null)
        {
            throw new CustomBasicException("The holdunhold showed nothing.  Rethink");
        }
        display.WillHold = !display.WillHold;
        StateChanged?.Invoke();
    }
    public bool CanNewRound => IsRoundOver;
    [Command(EnumCommandCategory.Plain)]
    public async Task NewRoundAsync()
    {
        await _mainGame.NewRoundAsync();
        StateChanged?.Invoke(); //hand needs to be refreshed here too.
    }
    
    void IMainGameVM.Initialize()
    {
        Subscribe();
        GlobalClass.PokerList.Clear(); //can't be brand new  that could cause the connection to break too.
        Round = 1;
        commandContainer.ExecutingChanged = CommandContainer_ExecutingChanged; //hopefully no problem (?)
        DeckPile = resolver.ReplaceObject<DeckObservablePile<PokerCardInfo>>();
        DeckPile.DeckClickedAsync = DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = true;
        DeckPile.SendEnableProcesses(this, () =>
        {
            return !IsRoundOver;
        });

        Bet1 = new NumberPicker(commandContainer, resolver);
        Bet1.SendEnableProcesses(this, () =>
        {
            return !BetPlaced;
        });
        Bet1.LoadNumberList([5, 10, 25]);
        Bet1.SelectNumberValue(5); //something else has to set to large (?)
        Bet1.ChangedNumberValueAsync = Bet1_ChangedNumberValueAsync;
        _mainGame = resolver.ReplaceObject<PokerMainGameClass>(); //hopefully this works.  means you have to really rethink.
        CreateCommands(commandContainer);
    }

    partial void CreateCommands(CommandContainer container);
    private async Task DeckPile_DeckClickedAsync()
    {
        _mainGame!.DrawFromDeck();
        await Task.CompletedTask;
    }
    private async void CommandContainer_ExecutingChanged()
    {
        if (commandContainer.IsExecuting)
        {
            return;
        }
        //code to run when its not busy.
        if (_mainGame.GameGoing)
        {
            await _mainGame.SaveStateAsync();
        }
    }
    private Task Bet1_ChangedNumberValueAsync(int chosen)
    {
        BetAmount = chosen;
        return Task.CompletedTask;
    }
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    
    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        basicData.GameDataLoading = false;
        await _mainGame.NewGameAsync(this);
    }

    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        commandContainer.ExecutingChanged = null;
        Unsubscribe();
    }

    void IHandle<NewCardsEventModel>.Handle(NewCardsEventModel message)
    {
        StateChanged?.Invoke();
    }
}