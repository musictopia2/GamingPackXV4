namespace AccordianSolitaire.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class AccordianSolitaireMainViewModel(CommandContainer commandContainer,

    IEventAggregator aggregator,
    IGamePackageResolver resolver,
        BasicData basicData,
        IToast toast
        ) : IBasicEnableProcess,
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>, IHandle<ScoreEventModel>
{
    private partial void Subscribe();
    private partial void Unsubscribe();
    private AccordianSolitaireMainGameClass _mainGame = null!;
    [StateProperty]
    public partial int Score { get; set; }
    public DeckObservablePile<AccordianSolitaireCardInfo> DeckPile { get; set; } = null!;
    public GameBoard GameBoard1 = null!;
    public void UnselectAll()
    {
        GameBoard1.UnselectAllObjects();
    }
    void IMainGameVM.Initialize()
    {
        Subscribe();
        commandContainer.ExecutingChanged = CommandContainer_ExecutingChanged; //hopefully no problem (?)
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
    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        basicData.GameDataLoading = true;
        await _mainGame.NewGameAsync(DeckPile, GameBoard1);
        GameBoard1.IsEnabled = true;
        basicData.GameDataLoading = false;
    }

    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
        commandContainer.ExecutingChanged = null;
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
            toast.ShowUserErrorToast("Illegal Move");
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
        return true;
    }
    void IHandle<ScoreEventModel>.Handle(ScoreEventModel message)
    {
        Score = message.Score;
    }
}