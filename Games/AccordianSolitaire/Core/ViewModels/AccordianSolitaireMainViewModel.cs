namespace AccordianSolitaire.Core.ViewModels;
[InstanceGame]
public partial class AccordianSolitaireMainViewModel : ScreenViewModel,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer,
        IHandle<ScoreEventModel>
{
    private readonly BasicData _basicData;
    private readonly IToast _toast;
    private readonly AccordianSolitaireMainGameClass _mainGame;
    public int Score { get; set; }
    public DeckObservablePile<AccordianSolitaireCardInfo> DeckPile { get; set; }
    public GameBoard GameBoard1;
    public void UnselectAll()
    {
        GameBoard1.UnselectAllObjects();
    }
    public AccordianSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        BasicData basicData,
        IToast toast
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        _toast = toast;
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged; //hopefully no problem (?)
        DeckPile = resolver.ReplaceObject<DeckObservablePile<AccordianSolitaireCardInfo>>();
        DeckPile.DeckClickedAsync = DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = true;
        DeckPile.SendEnableProcesses(this, () =>
        {
            return false;
        });
        GameBoard1 = new GameBoard(commandContainer);
        GameBoard1.ObjectClickedAsync = GameBoard1_ObjectClickedAsync;
        _mainGame = resolver.ReplaceObject<AccordianSolitaireMainGameClass>(); //hopefully this works.  means you have to really rethink.
    }
    private async Task GameBoard1_ObjectClickedAsync(AccordianSolitaireCardInfo card, int index)
    {
        if (index == -1)
        {
            throw new CustomBasicException("Index cannot be -1.  Rethink");
        }
        if (GameBoard1!.IsCardSelected(card) == false)
        {
            GameBoard1.SelectUnselectCard(card);
            return;
        }
        if (GameBoard1.IsValidMove(card) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        GameBoard1.MakeMove(card);
        if (Score == 52)
        {
            await _mainGame!.ShowWinAsync();
        }
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
        return true;
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = true;
        await base.ActivateAsync();
        await _mainGame.NewGameAsync(DeckPile, GameBoard1);
        GameBoard1.IsEnabled = true;
        _basicData.GameDataLoading = false;
    }
    void IHandle<ScoreEventModel>.Handle(ScoreEventModel message)
    {
        Score = message.Score;
    }
}