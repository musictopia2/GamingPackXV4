namespace Poker.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class PokerMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
{
    private readonly BasicData _basicData;
    private readonly PokerMainGameClass _mainGame;
    public static BasicList<DisplayCard> PokerList => GlobalClass.PokerList;
    public DeckObservablePile<PokerCardInfo> DeckPile { get; set; }
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
    public static void PopulateNewCards(IDeckDict<PokerCardInfo> thisList)
    {
        DisplayCard thisDisplay;
        if (PokerList.Count == 0)
        {
            BasicList<DisplayCard> newList = new();
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
    }
    [LabelColumn]
    public decimal Money { get; set; }
    public int BetAmount { get; set; } = 5;
    [LabelColumn]
    public decimal Winnings { get; set; }
    [LabelColumn]
    public string HandLabel { get; set; } = "";
    [LabelColumn]
    public int Round { get; set; }
    public bool BetPlaced { get; set; }
    public bool IsRoundOver { get; set; }
    public NumberPicker Bet1; //decided to have the numberpicker here.  because not only for new game but new round.
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
    public static void HoldUnhold(DisplayCard display)
    {
        if (display == null)
        {
            throw new CustomBasicException("The holdunhold showed nothing.  Rethink");
        }
        display.WillHold = !display.WillHold;
    }
    public bool CanNewRound => IsRoundOver;
    [Command(EnumCommandCategory.Plain)]
    public async Task NewRoundAsync()
    {
        await _mainGame.NewRoundAsync();
    }
    public PokerMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        BasicData basicData
        ) : base(aggregator)
    {
        GlobalClass.PokerList.Clear(); //can't be brand new  that could cause the connection to break too.
        Round = 1;
        CommandContainer = commandContainer;
        _basicData = basicData;
        CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
        DeckPile = resolver.ReplaceObject<DeckObservablePile<PokerCardInfo>>();
        DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
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
        Bet1.LoadNumberList(new BasicList<int>() { 5, 10, 25 });
        Bet1.SelectNumberValue(5); //something else has to set to large (?)
        Bet1.ChangedNumberValueAsync += Bet1_ChangedNumberValueAsync;
        _mainGame = resolver.ReplaceObject<PokerMainGameClass>(); //hopefully this works.  means you have to really rethink.
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    private async Task DeckPile_DeckClickedAsync()
    {
        _mainGame!.DrawFromDeck();
        await Task.CompletedTask;
    }
    private async void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer.IsExecuting)
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
    public CommandContainer CommandContainer { get; set; }
    IEventAggregator IAggregatorContainer.Aggregator => Aggregator;
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = false;
        await base.ActivateAsync();
        await _mainGame.NewGameAsync(this);
        CommandContainer.UpdateAll();
    }
    protected override Task TryCloseAsync()
    {
        CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
        return base.TryCloseAsync();
    }
}