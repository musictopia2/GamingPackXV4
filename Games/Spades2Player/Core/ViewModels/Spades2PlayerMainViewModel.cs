namespace Spades2Player.Core.ViewModels;
[InstanceGame]
public class Spades2PlayerMainViewModel : BasicCardGamesVM<Spades2PlayerCardInformation>
{
    private readonly Spades2PlayerMainGameClass _mainGame;
    private readonly Spades2PlayerVMData _model;
    private readonly IGamePackageResolver _resolver;
    private readonly Spades2PlayerGameContainer _gameContainer;
    public Spades2PlayerMainViewModel(CommandContainer commandContainer,
        Spades2PlayerMainGameClass mainGame,
        Spades2PlayerVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        Spades2PlayerGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _resolver = resolver;
        _gameContainer = gameContainer;
        _model.Deck1.NeverAutoDisable = true;
        _model.ChangeScreen = ScreenChangeAsync;
    }
    public SpadesBeginningViewModel? BeginningScreen { get; set; }
    public SpadesBiddingViewModel? BiddingScreen { get; set; }
    protected override Task ActivateAsync()
    {
        ScreenChangeAsync();
        return base.ActivateAsync();
    }
    private async void ScreenChangeAsync()
    {
        if (_isClosed)
        {
            return;
        }
        if (_model == null)
        {
            return;
        }
        if (_processing)
        {
            return;
        }
        _processing = true;
        if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.Normal)
        {
            await CloseBeginningScreenAsync();
            await CloseBiddingScreenAsync();
            _model.TrickArea1.Visible = true;
            _processing = false;
            return;
        }
        _model.TrickArea1.Visible = false;
        if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.Bidding)
        {
            await CloseBeginningScreenAsync();
            await OpenBiddingAsync();
            _processing = false;
            return;
        }
        if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.ChooseCards)
        {
            await CloseBiddingScreenAsync();
            await OpenBeginningAsync();
            _processing = false;
            return;
        }
        throw new CustomBasicException("Not supported.  Rethink");
    }
    private async Task CloseBeginningScreenAsync()
    {
        if (BeginningScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(BeginningScreen);
        BeginningScreen = null;
    }
    private async Task CloseBiddingScreenAsync()
    {
        if (BiddingScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(BiddingScreen);
        BiddingScreen = null;
    }
    private async Task OpenBeginningAsync()
    {
        if (BeginningScreen != null)
        {
            return;
        }
        BeginningScreen = _resolver.Resolve<SpadesBeginningViewModel>();
        await LoadScreenAsync(BeginningScreen);
    }
    private async Task OpenBiddingAsync()
    {
        if (BiddingScreen != null)
        {
            return;
        }
        BiddingScreen = _resolver.Resolve<SpadesBiddingViewModel>();
        await LoadScreenAsync(BiddingScreen);
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return _mainGame!.SaveRoot!.GameStatus == EnumGameStatus.ChooseCards;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        _model.OtherPile!.Visible = false;
        await _gameContainer!.SendDiscardMessageAsync(_model.OtherPile!.GetCardInfo().Deck);
        await _mainGame!.DiscardAsync(_model.OtherPile.GetCardInfo().Deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }

    private bool _processing;
    private bool _isClosed = false;
    protected override async Task TryCloseAsync()
    {
        _isClosed = true;
        await CloseBeginningScreenAsync();
        await CloseBiddingScreenAsync();
        await base.TryCloseAsync();
    }
}