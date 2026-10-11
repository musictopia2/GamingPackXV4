namespace PyramidSolitaire.Core.ViewModels;
[InstanceGame]
public partial class PyramidSolitaireMainViewModel : ScreenViewModel,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer,
        IHandle<MoveEventModel>,
        IHandle<ScoreEventModel>,
        ITriangleVM
{
    private readonly BasicData _basicData;
    private readonly IToast _toast;
    private readonly IMessageBox _message;
    private readonly PyramidSolitaireMainGameClass _mainGame;
    public DeckObservablePile<SolitaireCard> DeckPile { get; set; }
    public int Score { get; set; }
    public PlayList PlayList1;
    public TriangleBoard GameBoard1;
    public SingleObservablePile<SolitaireCard> Discard;
    public SingleObservablePile<SolitaireCard> CurrentPile;
    [Command(EnumCommandCategory.Plain)]
    public async Task PlaySelectedCardsAsync()
    {
        if (_mainGame.HasPlayedCard() == false)
        {
            _toast.ShowUserErrorToast("Sorry, there is no card to play");
            return;
        }
        if (_mainGame.IsValidMove() == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            _mainGame.PutBack();
            return;
        }
        await _mainGame.PlayCardsAsync();
    }
    public PyramidSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        BasicData basicData,
        IToast toast,
        IMessageBox message
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        _toast = toast;
        _message = message;
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        DeckPile = resolver.ReplaceObject<DeckObservablePile<SolitaireCard>>();
        DeckPile.DeckClickedAsync = DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = true;
        DeckPile.SendEnableProcesses(this, () =>
        {
            if (_mainGame!.GameGoing == false)
            {
                return false;
            }
            return true; //if other logic is needed for deck, put here.
        });
        _mainGame = resolver.ReplaceObject<PyramidSolitaireMainGameClass>(); //hopefully this works.  means you have to really rethink.
        CurrentPile = new SingleObservablePile<SolitaireCard>(commandContainer);
        CurrentPile.SendEnableProcesses(this, () => CurrentPile.PileEmpty() == false);
        CurrentPile.Text = "Current";
        CurrentPile.CurrentOnly = true;
        CurrentPile.PileClickedAsync = CurrentPile_PileClickedAsync;
        Discard = new SingleObservablePile<SolitaireCard>(CommandContainer);
        Discard.SendEnableProcesses(this, () => Discard.PileEmpty() == false);
        Discard.Text = "Discard";
        Discard.PileClickedAsync = Discard_PileClickedAsync;
        PlayList1 = new PlayList(CommandContainer);
        PlayList1.SendEnableProcesses(this, () => PlayList1.HasChosenCards());
        PlayList1.Visible = true;
        GameBoard1 = new TriangleBoard(this, CommandContainer, _mainGame);
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    private async Task DeckPile_DeckClickedAsync()
    {
        await _mainGame!.DrawCardAsync();
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
        DeckPile.IsEnabled = true;
        CommandContainer.UpdateAll();
    }
    void IHandle<MoveEventModel>.Handle(MoveEventModel message)
    {
        GameBoard1.MakeInvisible(message.Deck);
    }
    void IHandle<ScoreEventModel>.Handle(ScoreEventModel message)
    {
        Score = message.Score;
    }
    private async Task Discard_PileClickedAsync()
    {
        if (Discard!.PileEmpty())
        {
            throw new CustomBasicException("Since there is no card here, should have been disabled");
        }
        if (PlayList1!.AlreadyHasTwoCards())
        {
            await _message.ShowMessageAsync("Sorry, 2 has already been selected");
            return;
        }
        var thisCard = Discard.GetCardInfo();
        Discard.RemoveFromPile();
        PlayList1!.AddCard(thisCard);
        await Task.CompletedTask;
    }
    private async Task CurrentPile_PileClickedAsync()
    {
        if (CurrentPile!.PileEmpty())
        {
            throw new CustomBasicException("Since there is no card here, should have been disabled");
        }
        if (PlayList1!.AlreadyHasTwoCards())
        {
            await _message.ShowMessageAsync("Sorry, 2 has already been selected");
            return;
        }
        var thisCard = CurrentPile.GetCardInfo();
        CurrentPile.RemoveFromPile();
        PlayList1!.AddCard(thisCard);
        await Task.CompletedTask;
    }
    async Task ITriangleVM.CardClickedAsync(SolitaireCard thisCard)
    {
        if (PlayList1!.AlreadyHasTwoCards())
        {
            await _message.ShowMessageAsync("Sorry, 2 has already been selected");
            return;
        }
        PlayList1.AddCard(thisCard);
        GameBoard1!.MakeInvisible(thisCard.Deck);
    }
}