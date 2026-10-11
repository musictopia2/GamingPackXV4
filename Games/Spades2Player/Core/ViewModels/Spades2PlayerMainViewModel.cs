namespace Spades2Player.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class Spades2PlayerMainViewModel(CommandContainer commandContainer,
        Spades2PlayerMainGameClass mainGame,
        Spades2PlayerVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        Spades2PlayerGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<Spades2PlayerCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.ChangeScreen = ScreenChangeAsync;
        base.AdditionalInitialize();
    }
    [StateProperty]
    public partial SpadesBeginningViewModel? BeginningScreen { get; set; }
    [StateProperty]
    public partial SpadesBiddingViewModel? BiddingScreen { get; set; }
    protected override Task OpenAsync()
    {
        ScreenChangeAsync();
        return base.OpenAsync();
    }
    private void ScreenChangeAsync()
    {
        if (_isClosed)
        {
            return;
        }
        if (viewModel == null)
        {
            return;
        }
        if (_processing)
        {
            return;
        }
        _processing = true;
        if (mainGame.SaveRoot.GameStatus == EnumGameStatus.Normal)
        {
            CloseBeginningScreen();
            CloseBiddingScreen();
            viewModel.TrickArea1.Visible = true;
            _processing = false;
            return;
        }
        viewModel.TrickArea1.Visible = false;
        if (mainGame.SaveRoot.GameStatus == EnumGameStatus.Bidding)
        {
            CloseBeginningScreen();
            OpenBidding();
            _processing = false;
            return;
        }
        if (mainGame.SaveRoot.GameStatus == EnumGameStatus.ChooseCards)
        {
            CloseBiddingScreen();
            OpenBeginning();
            _processing = false;
            return;
        }
        throw new CustomBasicException("Not supported.  Rethink");
    }
    private void CloseBeginningScreen()
    {
        if (BeginningScreen == null)
        {
            return;
        }
        BeginningScreen = null;
    }
    private void CloseBiddingScreen()
    {
        if (BiddingScreen == null)
        {
            return;
        }
        BiddingScreen = null;
    }
    private void OpenBeginning()
    {
        if (BeginningScreen != null)
        {
            return;
        }
        BeginningScreen = Resolver.Resolve<SpadesBeginningViewModel>();
    }
    private void OpenBidding()
    {
        if (BiddingScreen != null)
        {
            return;
        }
        BiddingScreen = Resolver.Resolve<SpadesBiddingViewModel>();
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return mainGame!.SaveRoot!.GameStatus == EnumGameStatus.ChooseCards;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        viewModel.OtherPile!.Visible = false;
        await gameContainer!.SendDiscardMessageAsync(viewModel.OtherPile!.GetCardInfo().Deck);
        await mainGame!.DiscardAsync(viewModel.OtherPile.GetCardInfo().Deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }

    private bool _processing;
    private bool _isClosed = false;
    protected override Task CloseAsync()
    {
        _isClosed = true;
        CloseBeginningScreen();
        CloseBiddingScreen();
        return base.CloseAsync();
    }
}