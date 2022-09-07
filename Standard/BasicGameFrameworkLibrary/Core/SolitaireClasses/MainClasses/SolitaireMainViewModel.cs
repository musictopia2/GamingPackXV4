namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.MainClasses;
public abstract partial class SolitaireMainViewModel<S> : ScreenViewModel,
    IBasicSolitaireVM,
    IBasicEnableProcess,
    IBlankGameVM,
    IAggregatorContainer,
    IHandle<IScoreData>
    where S : SolitaireSavedClass, new()
{
    private SolitaireGameClass<S>? _mainGame;
    private int _score;
    [LabelColumn]
    public int Score
    {
        get { return _score; }
        set
        {
            if (SetProperty(ref _score, value))
            {
                if (_mainGame!.SaveRoot == null)
                {
                    return;
                }
                _mainGame.SaveRoot.Score = value;
            }
        }
    }
    public bool CanEnableBasics()
    {
        return true;
    }
    public bool CanStartNewGameImmediately { get; set; } = true;
    public DeckObservablePile<SolitaireCard> DeckPile { get; set; }
    public SingleObservablePile<SolitaireCard> MainDiscardPile { get; set; }
    public IMain MainPiles1 { get; set; }
    public IWaste WastePiles1 { get; set; }
    public CommandContainer CommandContainer { get; set; }
    private readonly BasicData _basicData;
    public SolitaireMainViewModel(IEventAggregator aggregator, CommandContainer command,
        IGamePackageResolver resolver
        ) : base(aggregator)
    {

        CommandContainer = command;
        _resolver = resolver;
        _ = resolver.ReplaceObject<IScoreData>();
        CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        DeckPile = resolver.ReplaceObject<DeckObservablePile<SolitaireCard>>();
        DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = true;
        _basicData = _resolver.Resolve<BasicData>();
        DeckPile.SendEnableProcesses(this, () =>
        {
            if (_mainGame!.GameGoing == false)
            {
                return false;
            }
            if (_mainGame.DealsRemaining == 0 && DeckPile.IsEndOfDeck())
            {
                return false;
            }
            else if (_mainGame.NoCardsToShuffle)
            {
                return false;
            }
            return true;
        });
        MainDiscardPile = new SingleObservablePile<SolitaireCard>(command);
        MainDiscardPile.PileClickedAsync += MainDiscardPile_PileClickedAsync;
        MainPiles1 = resolver.ReplaceObject<IMain>();
        WastePiles1 = resolver.ReplaceObject<IWaste>();
        MainPiles1.PileSelectedAsync += MainPiles1_PileSelectedAsync;
        WastePiles1.PileSelectedAsync += WastePiles1_PileSelectedAsync;
        WastePiles1.DoubleClickAsync += WastePiles1_DoubleClickAsync;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    private async Task DeckPile_DeckClickedAsync()
    {
        await Task.CompletedTask;
        if (DeckPile!.DeckStyle == EnumDeckPileStyle.AlwaysKnown)
        {
            if (WastePiles1!.OneSelected() > 0)
            {
                return;
            }
            DeckPile.IsSelected = !DeckPile.IsSelected;
            return;
        }
        _mainGame!.DrawCard();
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task AutoMoveAsync()
    {
        await _mainGame!.MakeAutoMovesToMainPilesAsync();
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = true;
        _mainGame = GetGame(_resolver);
        _mainGame.LinkData(this);
        await _mainGame.InitAsync(this);
        CommandContainer.ManualReport();
        CommandContainer.UpdateAll();
        _basicData.GameDataLoading = false;
    }
    protected abstract SolitaireGameClass<S> GetGame(IGamePackageResolver resolver);
    private async Task MainPiles1_PileSelectedAsync(int index)
    {
        await _mainGame!.MainPileSelectedAsync(index);
    }
    private bool _didDoubleClick;
    private readonly IGamePackageResolver _resolver;
    private async Task WastePiles1_DoubleClickAsync(int Index)
    {
        _didDoubleClick = true;
        await _mainGame!.WasteToMainAsync(Index);
    }
    private async Task WastePiles1_PileSelectedAsync(int Index)
    {
        if (_didDoubleClick == true)
        {
            _didDoubleClick = false;
            return;
        }
        await _mainGame!.WasteSelectedAsync(Index);
    }
    private async Task MainDiscardPile_PileClickedAsync()
    {
        await Task.CompletedTask;
        _mainGame!.SelectDiscard();
    }
    private async void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer!.IsExecuting)
        {
            return;
        }
        if (_mainGame == null || _mainGame.SaveRoot == null || _mainGame.GameGoing == false)
        {
            return;
        }
        CommandExecutingChanged();
        await _mainGame.SaveGameAsync();
    }
    protected virtual void CommandExecutingChanged() { }
    protected override Task TryCloseAsync()
    {
        CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
        return base.TryCloseAsync();
    }
    void IHandle<IScoreData>.Handle(IScoreData message)
    {
        Score = message.Score;
    }
}