namespace Blackjack.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class BlackjackMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
{
    private readonly BlackjackMainGameClass _mainGame;
    public DeckObservablePile<BlackjackCardInfo> DeckPile { get; set; }
    public BlackjackMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        StatsClass stats,
        BasicData basicData
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
        DeckPile = resolver.ReplaceObject<DeckObservablePile<BlackjackCardInfo>>();
        DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
        _stats = stats;
        _basicData = basicData;
        HumanStack = new PlayerStack(commandContainer);
        ComputerStack = new PlayerStack(commandContainer);
        HumanStack.ProcessLabel(false);
        HumanStack.CardSelectedAsync += HumanStack_CardSelectedAsync;
        ComputerStack.ProcessLabel(true);
        ComputerStack.AlwaysDisabled = true;
        HumanStack.SendFunction(() => NeedsAceChoice == false && SelectedYet == false);
        Wins = stats.Wins;
        Losses = stats.Losses;
        Draws = stats.Draws;
        DeckPile.NeverAutoDisable = true;
        DeckPile.IsEnabled = false;
        DeckPile.SendEnableProcesses(this, () =>
        {
            return false; //false this time.
        });
        _mainGame = resolver.ReplaceObject<BlackjackMainGameClass>(); //hopefully this works.  means you have to really rethink.
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    private readonly StatsClass _stats;
    private readonly BasicData _basicData;

    public PlayerStack? ComputerStack; //decided to make it more clear now.
    public PlayerStack? HumanStack;
    private bool _needsAceChoice;
    public bool NeedsAceChoice
    {
        get
        {
            return _needsAceChoice;
        }

        set
        {
            if (SetProperty(ref _needsAceChoice, value) == true)
            {
                Reprocess();
            }
        }
    }
    private bool _selectedYet;
    public bool SelectedYet
    {
        get { return _selectedYet; }
        set
        {
            if (SetProperty(ref _selectedYet, value))
            {
                Reprocess();
            }

        }
    }
    private void Reprocess()
    {
        if (NeedsAceChoice == false && SelectedYet == true)
        {
            CanHitOrStay = true;
        }
        else
        {
            CanHitOrStay = false;
        }
    }
    public bool CanHitOrStay { get; set; }
    [LabelColumn]
    public int HumanPoints { get; set; }
    [LabelColumn]
    public int ComputerPoints { get; set; }
    private int _draws;
    [LabelColumn]
    public int Draws
    {
        get
        {
            return _draws;
        }

        set
        {
            if (SetProperty(ref _draws, value) == true)
            {
                _stats.Draws = value;
            }
        }
    }
    private int _wins;
    [LabelColumn]
    public int Wins
    {
        get
        {
            return _wins;
        }

        set
        {
            if (SetProperty(ref _wins, value) == true)
            {
                _stats.Wins = value;
            }
        }
    }
    private int _losses;
    [LabelColumn]
    public int Losses
    {
        get
        {
            return _losses;
        }

        set
        {
            if (SetProperty(ref _losses, value) == true)
            {
                _stats.Losses = value;
            }
        }
    }
    public bool CanAce => NeedsAceChoice;
    [Command(EnumCommandCategory.Game)]
    public async Task AceAsync(EnumAceChoice choice)
    {
        await _mainGame.HumanAceAsync(choice);
    }
    public bool CanHit => CanHitOrStay;
    [Command(EnumCommandCategory.Game)]
    public async Task HitAsync()
    {
        await _mainGame.HumanHitAsync();
    }
    public bool CanStay => CanHitOrStay;
    [Command(EnumCommandCategory.Game)]
    public async Task StayAsync()
    {
        await _mainGame.HumanStayAsync();
    }

    private async Task DeckPile_DeckClickedAsync()
    {
        //if we click on deck, will do code for this.
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
    private async Task HumanStack_CardSelectedAsync(bool HasChoice)
    {
        await _mainGame!.HumanSelectAsync(HasChoice);
    }
    public CommandContainer CommandContainer { get; set; }
    IEventAggregator IAggregatorContainer.Aggregator => Aggregator;
    public bool CanEnableBasics()
    {
        return true;
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = false; //because no autoresume.
        await base.ActivateAsync();
        await _mainGame.NewGameAsync(DeckPile, this);
        CommandContainer.ManualReport(); //just had to run the code to manually report.
        CommandContainer.UpdateAll();
    }
    protected override Task TryCloseAsync()
    {
        CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
        return base.TryCloseAsync();
    }
}