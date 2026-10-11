namespace ClockSolitaire.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class ClockSolitaireMainViewModel(CommandContainer commandContainer,

    IEventAggregator aggregator,
    IGamePackageResolver resolver,
        BasicData basicData,
        IToast toast
        ) : IBasicEnableProcess,
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>,
        IClockVM,
        IHandle<CardsLeftEventModel>
{
    private ClockSolitaireMainGameClass _mainGame = null!;
    [StateProperty]
    public partial int CardsLeft { get; set; }
    public DeckObservablePile<SolitaireCard> DeckPile { get; set; } = null!;
    public ClockBoard? Clock1;
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IMainGameVM.Initialize()
    {
        Subscribe();
        commandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        DeckPile = resolver.ReplaceObject<DeckObservablePile<SolitaireCard>>();
        DeckPile.DeckClickedAsync = DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = true;
        DeckPile.SendEnableProcesses(this, () =>
        {
            return false; //i think.
        });
        _mainGame = resolver.ReplaceObject<ClockSolitaireMainGameClass>();
        Clock1 = new ClockBoard(this, _mainGame, commandContainer, aggregator);
    }
    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {

        basicData.GameDataLoading = true;
        await _mainGame.NewGameAsync(this);
        basicData.GameDataLoading = false;
    }

    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
    }
    private async Task DeckPile_DeckClickedAsync()
    {
        await Task.CompletedTask;
    }
    private async void CommandContainer_ExecutingChanged()
    {
        if (commandContainer.IsExecuting)
        {
            return;
        }
        if (_mainGame.GameGoing)
        {
            await _mainGame.SaveStateAsync();
        }
    }
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    async Task IClockVM.ClockClickedAsync(int index)
    {
        if (Clock1!.IsValidMove(index) == false)
        {
            toast.ShowUserErrorToast("Illegal Move");
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