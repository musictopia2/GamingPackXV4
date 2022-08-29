namespace OldMaid.Core.ViewModels;
[InstanceGame]
public class OldMaidMainViewModel : BasicCardGamesVM<RegularSimpleCard>
{
    private readonly OldMaidMainGameClass _mainGame;
    private readonly OldMaidVMData _model;
    private readonly IGamePackageResolver _resolver;
    private readonly IToast _toast;
    public OldMaidMainViewModel(CommandContainer commandContainer,
        OldMaidMainGameClass mainGame,
        OldMaidVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        OldMaidGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _resolver = resolver;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = false;
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        gameContainer.ShowOtherCardsAsync = LoadOpponentScreenAsync;
    }
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        if (_mainGame.SaveRoot.RemovePairs == false)
        {
            await LoadOpponentScreenAsync();
        }
    }
    private async Task LoadOpponentScreenAsync()
    {
        if (OpponentScreen != null)
        {
            return;
        }
        OpponentScreen = _resolver.Resolve<OpponentCardsViewModel>();
        await LoadScreenAsync(OpponentScreen);
    }
    protected override async Task TryCloseAsync()
    {
        if (OpponentScreen != null)
        {
            await CloseSpecificChildAsync(OpponentScreen);
            OpponentScreen = null;
        }
        await base.TryCloseAsync();
    }
    public OpponentCardsViewModel? OpponentScreen { get; set; }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        var thisCol = _model.PlayerHand1!.ListSelectedObjects();
        if (thisCol.Count != 2)
        {
            _toast.ShowUserErrorToast("Must select 2 cards to throw away");
            return;
        }
        if (OldMaidMainGameClass.IsValidMove(thisCol) == false)
        {
            _toast.ShowUserErrorToast("Illegal move");
            return;
        }
        await _mainGame.ProcessPlayAsync(thisCol.First().Deck, thisCol.Last().Deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}