namespace Uno.Core.ViewModels;
[InstanceGame]
public class UnoMainViewModel : BasicCardGamesVM<UnoCardInformation>
{
    private readonly UnoMainGameClass _mainGame;
    private readonly UnoVMData _model;
    private readonly IGamePackageResolver _resolver;
    private readonly UnoGameContainer _gameContainer;
    private readonly IToast _toast;
    public SayUnoViewModel? SayUnoScreen { get; set; }
    public UnoMainViewModel(CommandContainer commandContainer,
        UnoMainGameClass mainGame,
        UnoVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        UnoGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _resolver = resolver;
        _gameContainer = gameContainer;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        _gameContainer.OpenSaidUnoAsync = LoadSayUnoAsync;
        _gameContainer.CloseSaidUnoAsync = CloseSayUnoAsync;
    }
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        if (_gameContainer.SaveRoot.GameStatus == EnumGameStatus.WaitingForUno)
        {
            await LoadSayUnoAsync();
        }
    }
    protected override async Task TryCloseAsync()
    {
        await CloseSayUnoAsync();
        await base.TryCloseAsync();
    }
    private async Task LoadSayUnoAsync()
    {
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            return;
        }
        SayUnoScreen = _resolver.Resolve<SayUnoViewModel>();
        await LoadScreenAsync(SayUnoScreen);
    }
    private async Task CloseSayUnoAsync()
    {
        if (SayUnoScreen != null)
        {
            await CloseSpecificChildAsync(SayUnoScreen);
            SayUnoScreen = null;
        }
    }
    protected override bool CanEnableDeck()
    {
        if (_mainGame!.SaveRoot!.GameStatus != EnumGameStatus.NormalPlay)
        {
            return false;
        }
        if (_gameContainer.AlreadyDrew == true)
        {
            return false;
        }
        return CanDraw();
    }
    protected override bool CanEnablePile1()
    {
        return _mainGame!.SaveRoot!.GameStatus == EnumGameStatus.NormalPlay;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int deck = _model.PlayerHand1!.ObjectSelected();
        if (deck == 0)
        {
            _toast.ShowUserErrorToast("You must select a card first");
            return;
        }
        if (_mainGame!.CanPlay(deck) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            _model.PlayerHand1.UnselectAllObjects();
            return;
        }
        await _mainGame.ProcessPlayAsync(deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    private bool CanDraw()
    {
        return !_mainGame!.SingleInfo!.MainHandList.Any(x => _mainGame.CanPlay(x.Deck));
    }
    public override bool CanEndTurn()
    {
        if (_mainGame!.SaveRoot!.GameStatus != EnumGameStatus.NormalPlay)
        {
            return false;
        }
        if (CanDraw() == false)
        {
            return false;
        }
        return _gameContainer.AlreadyDrew;
    }
}