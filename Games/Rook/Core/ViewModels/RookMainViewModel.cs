namespace Rook.Core.ViewModels;
[InstanceGame]
public partial class RookMainViewModel : BasicCardGamesVM<RookCardInformation>
{
    private readonly RookMainGameClass _mainGame;
    public readonly RookVMData Model;
    //private readonly IGamePackageResolver _resolver;
    private readonly IToast _toast;
    private readonly INestProcesses _nestProcesses;
    private readonly IBidProcesses _bidProcesses;
    private readonly ITrumpProcesses _trumpProcesses;
    public RookMainViewModel(CommandContainer commandContainer,
        RookMainGameClass mainGame,
        RookVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast,
        INestProcesses nestProcesses,
        IBidProcesses bidProcesses,
        ITrumpProcesses trumpProcesses
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        Model = viewModel;
        //_resolver = resolver;
        _toast = toast;
        _nestProcesses = nestProcesses;
        _bidProcesses = bidProcesses;
        _trumpProcesses = trumpProcesses;
        Model.Deck1.NeverAutoDisable = true;
        //Model.ChangeScreen = ScreenChangeAsync;
        Model.Dummy1.SendEnableProcesses(this, () =>
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
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    //protected override async Task TryCloseAsync()
    //{
    //    await CloseBiddingScreenAsync();
    //    await CloseColorScreenAsync();
    //    await CloseNestScreenAsync();
    //    await base.TryCloseAsync();
    //}
    //public RookBiddingViewModel? BidScreen { get; set; }
    //public RookNestViewModel? NestScreen { get; set; }
    //public RookColorViewModel? ColorScreen { get; set; }
    //protected override Task ActivateAsync()
    //{
    //    ScreenChangeAsync();
    //    return base.ActivateAsync();
    //}
    //private async void ScreenChangeAsync()
    //{
    //    if (_mainGame == null)
    //    {
    //        return;
    //    }
    //    if (_mainGame.SaveRoot.GameStatus == EnumStatusList.Normal)
    //    {
    //        await CloseNestScreenAsync();
    //        await CloseBiddingScreenAsync();
    //        await CloseColorScreenAsync();
    //        Model.TrickArea1.Visible = true;
    //        return;
    //    }
    //    Model.TrickArea1.Visible = false;
    //    if (_mainGame.SaveRoot.GameStatus == EnumStatusList.Bidding)
    //    {
    //        await CloseNestScreenAsync();
    //        await CloseColorScreenAsync();
    //        await OpenBiddingAsync();
    //        return;
    //    }
    //    if (_mainGame.SaveRoot.GameStatus == EnumStatusList.SelectNest)
    //    {
    //        await CloseBiddingScreenAsync();
    //        await CloseColorScreenAsync();
    //        await OpenNestAsync();
    //        return;
    //    }
    //    if (_mainGame.SaveRoot.GameStatus == EnumStatusList.ChooseTrump)
    //    {
    //        await CloseBiddingScreenAsync();
    //        await CloseNestScreenAsync();
    //        await OpenColorAsync();
    //        return;
    //    }
    //    throw new CustomBasicException("Not supported.  Rethink");
    //}
    public EnumStatusList GameStatus => _mainGame.SaveRoot.GameStatus;
    //private async Task CloseBiddingScreenAsync()
    //{
    //    if (BidScreen == null)
    //    {
    //        return;
    //    }
    //    await CloseSpecificChildAsync(BidScreen);
    //    BidScreen = null;
    //}
    //private async Task CloseNestScreenAsync()
    //{
    //    if (NestScreen == null)
    //    {
    //        return;
    //    }
    //    await CloseSpecificChildAsync(NestScreen);
    //    NestScreen = null;
    //}
    //private async Task CloseColorScreenAsync()
    //{
    //    if (ColorScreen == null)
    //    {
    //        return;
    //    }
    //    await CloseSpecificChildAsync(ColorScreen);
    //    ColorScreen = null;
    //}
    //private async Task OpenBiddingAsync()
    //{
    //    if (BidScreen != null)
    //    {
    //        return;
    //    }
    //    BidScreen = _resolver.Resolve<RookBiddingViewModel>();
    //    await LoadScreenAsync(BidScreen);
    //}
    //private async Task OpenNestAsync()
    //{
    //    if (NestScreen != null)
    //    {
    //        return;
    //    }
    //    NestScreen = _resolver.Resolve<RookNestViewModel>();
    //    await LoadScreenAsync(NestScreen);
    //}
    //private async Task OpenColorAsync()
    //{
    //    if (ColorScreen != null)
    //    {
    //        return;
    //    }
    //    ColorScreen = _resolver.Resolve<RookColorViewModel>();
    //    await LoadScreenAsync(ColorScreen);
    //}
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
    [Command(EnumCommandCategory.Plain)]
    public async Task ChooseNestAsync()
    {
        var thisList = Model.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count != 5)
        {
            _toast.ShowUserErrorToast("Sorry, you must choose 5 cards to throw away");
            return;
        }
        await _nestProcesses.ProcessNestAsync(thisList);
    }
    public bool CanBid => Model.BidChosen > -1;
    [Command(EnumCommandCategory.Plain)]
    public async Task BidAsync()
    {
        await _bidProcesses.ProcessBidAsync();
    }
    public bool CanPass => Model.CanPass;
    [Command(EnumCommandCategory.Plain)]
    public async Task PassAsync()
    {
        await _bidProcesses.PassBidAsync();
    }
    public bool CanTrump => Model.ColorChosen != EnumColorTypes.None;
    [Command(EnumCommandCategory.Plain)]
    public async Task TrumpAsync()
    {
        await _trumpProcesses.ProcessTrumpAsync();
    }
}