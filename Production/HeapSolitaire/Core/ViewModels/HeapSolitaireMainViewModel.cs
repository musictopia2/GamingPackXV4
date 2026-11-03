namespace HeapSolitaire.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class HeapSolitaireMainViewModel(CommandContainer commandContainer,

    IEventAggregator aggregator,
    IGamePackageResolver resolver,
        BasicData basicData
        ) : IBasicEnableProcess,
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>, IHandle<ScoreEventModel>
{
    private partial void Subscribe();
    private partial void Unsubscribe();
    private HeapSolitaireMainGameClass _mainGame = null!;
    public DeckObservablePile<HeapSolitaireCardInfo> DeckPile { get; set; } = null!;
    public WastePiles Waste1 = null!;
    public MainPiles Main1 = null!;
    [StateProperty]
    public partial int Score { get; set; }
    void IMainGameVM.Initialize()
    {
        Subscribe();
        commandContainer.ExecutingChanged = CommandContainer_ExecutingChanged; //hopefully no problem (?)
        DeckPile = resolver.ReplaceObject<DeckObservablePile<HeapSolitaireCardInfo>>();
        DeckPile.DeckClickedAsync = DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = true;
        DeckPile.SendEnableProcesses(this, () =>
        {
            return false;
        });
        _mainGame = resolver.ReplaceObject<HeapSolitaireMainGameClass>();
        Waste1 = new WastePiles(commandContainer, _mainGame);
        Waste1.PileClickedAsync = Waste1_PileClickedAsync;
        Main1 = new MainPiles(commandContainer, _mainGame);
        Main1.PileClickedAsync = Main1_PileClickedAsync;
    }

    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        basicData.GameDataLoading = false;
        await _mainGame.NewGameAsync(this);
        Waste1.IsEnabled = true;
        Main1.IsEnabled = true;
    }
    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
        commandContainer.ExecutingChanged = null;
    }
    private async Task Main1_PileClickedAsync(int index, BasicPileInfo<HeapSolitaireCardInfo> pile)
    {
        await _mainGame!.SelectMainAsync(index);
    }
    private Task Waste1_PileClickedAsync(int index, BasicPileInfo<HeapSolitaireCardInfo> pile)
    {
        Waste1!.SelectPile(index);
        return Task.CompletedTask;
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
    void IHandle<ScoreEventModel>.Handle(ScoreEventModel message)
    {
        Score = message.Score;
    }
}