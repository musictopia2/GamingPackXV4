namespace HeapSolitaire.Core.ViewModels;
[InstanceGame]
public partial class HeapSolitaireMainViewModel : ScreenViewModel,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer,
        IHandle<ScoreEventModel>
{
    private readonly BasicData _basicData;
    private readonly HeapSolitaireMainGameClass _mainGame;
    public DeckObservablePile<HeapSolitaireCardInfo> DeckPile { get; set; }
    public WastePiles Waste1;
    public MainPiles Main1;
    public int Score { get; set; }
    public HeapSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        BasicData basicData
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
        DeckPile = resolver.ReplaceObject<DeckObservablePile<HeapSolitaireCardInfo>>();
        DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = true;
        DeckPile.SendEnableProcesses(this, () =>
        {
            return false;
        });
        _mainGame = resolver.ReplaceObject<HeapSolitaireMainGameClass>();
        Waste1 = new WastePiles(CommandContainer, _mainGame);
        Waste1.PileClickedAsync += Waste1_PileClickedAsync;
        Main1 = new MainPiles(CommandContainer, _mainGame);
        Main1.PileClickedAsync += Main1_PileClickedAsync;
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
        _basicData.GameDataLoading = false;
        await base.ActivateAsync();
        await _mainGame.NewGameAsync(this);
        Waste1.IsEnabled = true;
        Main1.IsEnabled = true;
        CommandContainer.UpdateAll();
    }
    protected override Task TryCloseAsync()
    {
        CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
        return base.TryCloseAsync();
    }
    void IHandle<ScoreEventModel>.Handle(ScoreEventModel message)
    {
        Score = message.Score;
    }
}