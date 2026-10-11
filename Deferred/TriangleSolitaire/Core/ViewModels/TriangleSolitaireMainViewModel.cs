namespace TriangleSolitaire.Core.ViewModels;
[InstanceGame]
public class TriangleSolitaireMainViewModel : ScreenViewModel,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer,
        ITriangleVM
{
    private readonly BasicData _basicData;
    private readonly IToast _toast;
    private readonly ISystemError _error;
    private readonly TriangleSolitaireMainGameClass _mainGame;
    public DeckObservablePile<SolitaireCard> DeckPile { get; set; }
    public SingleObservablePile<SolitaireCard> Pile1;
    public TriangleBoard Triangle1;
    public TriangleSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        BasicData basicData,
        IToast toast,
        ISystemError error
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        _toast = toast;
        _error = error;
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged; //hopefully no problem (?)
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
        _mainGame = resolver.ReplaceObject<TriangleSolitaireMainGameClass>(); //hopefully this works.  means you have to really rethink.
        Pile1 = new SingleObservablePile<SolitaireCard>(commandContainer);
        Triangle1 = new TriangleBoard(this, CommandContainer);
        Pile1.Text = "Discard";
        Pile1.SendEnableProcesses(this, () => false);
    }
    private async Task DeckPile_DeckClickedAsync()
    {
        if (DeckPile!.IsEndOfDeck())
        {
            _toast.ShowInfoToast($"You left {Triangle1!.HowManyCardsLeft} cards");
            await Task.Delay(2000);
            _mainGame.GameGoing = false;
            await _mainGame.SendGameOverAsync(_error);
            return;
        }
        _mainGame!.DrawCard(this);
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
    IEventAggregator IAggregatorContainer.Aggregator => Aggregator;
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    public Action? StateHasChanged { get; set; }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = false;
        await base.ActivateAsync();
        Pile1.ClearCards();
        _mainGame.InitDraw = (() =>
        {
            var newList = DeckPile!.DrawSeveralCards(15);
            Triangle1!.ClearCards(newList);
            _mainGame.DrawCard(this);
        });
        await _mainGame.NewGameAsync(DeckPile);
        DeckPile.IsEnabled = true; //has to be done manually.
        StateHasChanged?.Invoke();
    }
    async Task ITriangleVM.CardClickedAsync(SolitaireCard thisCard)
    {
        var pileCard = Pile1!.GetCardInfo();
        if (_mainGame!.IsValidMove(thisCard, pileCard, out EnumIncreaseList tempi) == false)
        {
            _toast.ShowUserErrorToast("Sorry, wrong card");
            return;
        }
        await _mainGame.MakeMoveAsync(thisCard.Deck, tempi, this);
    }
}