namespace ClockSolitaire.Core.ViewModels;
[InstanceGame]
public partial class ClockSolitaireMainViewModel : ScreenViewModel,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer,
        IClockVM,
        IHandle<CardsLeftEventModel>
{
    private readonly BasicData _basicData;
    private readonly IToast _toast;
    private readonly ClockSolitaireMainGameClass _mainGame;
    public int CardsLeft { get; set; }
    public DeckObservablePile<SolitaireCard> DeckPile { get; set; }
    public ClockBoard? Clock1;
    public ClockSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        BasicData basicData,
        IToast toast
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        _toast = toast;
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        DeckPile = resolver.ReplaceObject<DeckObservablePile<SolitaireCard>>();
        DeckPile.DeckClickedAsync = DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = true;
        DeckPile.SendEnableProcesses(this, () =>
        {
            return false; //i think.
        });
        _mainGame = resolver.ReplaceObject<ClockSolitaireMainGameClass>();
        Clock1 = new ClockBoard(this, _mainGame, commandContainer, Aggregator);
    }
    private async Task DeckPile_DeckClickedAsync()
    {
        await Task.CompletedTask;
    }
    private async void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer.IsExecuting)
        {
            return;
        }
        if (_mainGame.GameGoing)
        {
            await _mainGame.SaveStateAsync();
        }
    }
    public CommandContainer CommandContainer { get; set; }
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = true;
        await base.ActivateAsync();
        await _mainGame.NewGameAsync(this);
        CommandContainer.UpdateAll();
        _basicData.GameDataLoading = false;
    }
    async Task IClockVM.ClockClickedAsync(int index)
    {
        if (Clock1!.IsValidMove(index) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        Clock1.MakeMove(index);
        if (Clock1.HasWon())
        {
            await _mainGame!.ShowWinAsync();
            return;
        }
        if (Clock1.IsGameOver())
        {
            await _mainGame!.ShowLossAsync();
        }
    }
    void IHandle<CardsLeftEventModel>.Handle(CardsLeftEventModel message)
    {
        CardsLeft = message.CardsLeft;
    }
}