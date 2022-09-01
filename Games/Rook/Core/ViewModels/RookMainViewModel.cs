namespace Rook.Core.ViewModels;
[InstanceGame]
public class RookMainViewModel : BasicCardGamesVM<RookCardInformation>
{
    private readonly RookMainGameClass _mainGame;
    private readonly RookVMData _model;
    private readonly IGamePackageResolver _resolver;
    public RookMainViewModel(CommandContainer commandContainer,
        RookMainGameClass mainGame,
        RookVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _resolver = resolver;
        _model.Deck1.NeverAutoDisable = true;
        _model.ChangeScreen = ScreenChangeAsync;
        _model.Dummy1.SendEnableProcesses(this, () =>
        {
            if (_mainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
            {
                return false;
            }
            if (_mainGame.PlayerList.Count == 3)
            {
                return false;
            }
            return _mainGame.SaveRoot.DummyPlay;
        });
    }
    protected override async Task TryCloseAsync()
    {
        await CloseBiddingScreenAsync();
        await CloseColorScreenAsync();
        await CloseNestScreenAsync();
        await base.TryCloseAsync();
    }
    public RookBiddingViewModel? BidScreen { get; set; }
    public RookNestViewModel? NestScreen { get; set; }
    public RookColorViewModel? ColorScreen { get; set; }
    protected override Task ActivateAsync()
    {
        ScreenChangeAsync();
        return base.ActivateAsync();
    }
    private async void ScreenChangeAsync()
    {
        if (_mainGame == null)
        {
            return;
        }
        if (_mainGame.SaveRoot.GameStatus == EnumStatusList.Normal)
        {
            await CloseNestScreenAsync();
            await CloseBiddingScreenAsync();
            await CloseColorScreenAsync();
            _model.TrickArea1.Visible = true;
            return;
        }
        _model.TrickArea1.Visible = false;
        if (_mainGame.SaveRoot.GameStatus == EnumStatusList.Bidding)
        {
            await CloseNestScreenAsync();
            await CloseColorScreenAsync();
            await OpenBiddingAsync();
            return;
        }
        if (_mainGame.SaveRoot.GameStatus == EnumStatusList.SelectNest)
        {
            await CloseBiddingScreenAsync();
            await CloseColorScreenAsync();
            await OpenNestAsync();
            return;
        }
        if (_mainGame.SaveRoot.GameStatus == EnumStatusList.ChooseTrump)
        {
            await CloseBiddingScreenAsync();
            await CloseNestScreenAsync();
            await OpenColorAsync();
            return;
        }
        throw new CustomBasicException("Not supported.  Rethink");
    }
    private async Task CloseBiddingScreenAsync()
    {
        if (BidScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(BidScreen);
        BidScreen = null;
    }
    private async Task CloseNestScreenAsync()
    {
        if (NestScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(NestScreen);
        NestScreen = null;
    }
    private async Task CloseColorScreenAsync()
    {
        if (ColorScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(ColorScreen);
        ColorScreen = null;
    }
    private async Task OpenBiddingAsync()
    {
        if (BidScreen != null)
        {
            return;
        }
        BidScreen = _resolver.Resolve<RookBiddingViewModel>();
        await LoadScreenAsync(BidScreen);
    }
    private async Task OpenNestAsync()
    {
        if (NestScreen != null)
        {
            return;
        }
        NestScreen = _resolver.Resolve<RookNestViewModel>();
        await LoadScreenAsync(NestScreen);
    }
    private async Task OpenColorAsync()
    {
        if (ColorScreen != null)
        {
            return;
        }
        ColorScreen = _resolver.Resolve<RookColorViewModel>();
        await LoadScreenAsync(ColorScreen);
    }
    public override bool CanEndTurn()
    {
        return false;
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    protected override bool AlwaysEnableHand()
    {
        return false;
    }
    protected override bool CanEnableHand()
    {
        if (_mainGame!.SaveRoot!.GameStatus == EnumStatusList.SelectNest)
        {
            return true;
        }
        if (_mainGame.SaveRoot.GameStatus == EnumStatusList.Normal)
        {
            return !_mainGame.SaveRoot.DummyPlay;
        }
        return false;
    }
}