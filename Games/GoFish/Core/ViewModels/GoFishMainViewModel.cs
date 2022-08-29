namespace GoFish.Core.ViewModels;
[InstanceGame]
public class GoFishMainViewModel : BasicCardGamesVM<RegularSimpleCard>
{
    private readonly GoFishMainGameClass _mainGame;
    private readonly GoFishVMData _model;
    private readonly IGamePackageResolver _resolver;
    private readonly IToast _toast;
    public GoFishMainViewModel(CommandContainer commandContainer,
        GoFishMainGameClass mainGame,
        GoFishVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        GoFishGameContainer gameContainer,
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
        gameContainer.LoadAskScreenAsync = LoadAskScreenAsync;
    }
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        if (_mainGame.SaveRoot.RemovePairs == false)
        {
            await LoadAskScreenAsync();
        }
    }
    private async Task LoadAskScreenAsync()
    {
        if (AskScreen != null)
        {
            return;
        }
        AskScreen = _resolver.Resolve<AskViewModel>();
        await LoadScreenAsync(AskScreen);
    }
    protected override async Task TryCloseAsync()
    {
        if (AskScreen != null)
        {
            await CloseSpecificChildAsync(AskScreen);
            AskScreen = null;
        }
        await base.TryCloseAsync();
    }
    public AskViewModel? AskScreen { get; set; }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        return _mainGame!.SaveRoot!.RemovePairs == true || _mainGame.SaveRoot.NumberAsked == true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        var thisList = _model.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count != 2)
        {
            _toast.ShowUserErrorToast("Must select 2 cards to throw away");
            return;
        }
        if (_mainGame!.IsValidMove(thisList) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            SendPair thisPair = new();
            thisPair.Card1 = thisList.First().Deck;
            thisPair.Card2 = thisList.Last().Deck;
            await _mainGame.Network!.SendAllAsync("processplay", thisPair);
        }
        await _mainGame.ProcessPlayAsync(thisList.First().Deck, thisList.Last().Deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}