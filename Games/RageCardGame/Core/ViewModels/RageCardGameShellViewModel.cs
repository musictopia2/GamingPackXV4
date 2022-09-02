namespace RageCardGame.Core.ViewModels;
public class RageCardGameShellViewModel : BasicTrickShellViewModel<RageCardGamePlayerItem>
{
    public RageCardGameShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        RageDelgates delgates,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        delgates.CloseBidScreenAsync = CloseBidScreenAsync;
        delgates.LoadBidScreenAsync = LoadBidScreenAsync;
        delgates.CloseColorScreenAsync = CloseColorScreenAsync;
        delgates.LoadColorScreenAsync = LoadColorScreenAsync;
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<RageCardGameMainViewModel>();
        return model;
    }
    public async Task LoadColorScreenAsync()
    {
        if (ColorScreen != null)
        {
            return;
        }
        //await CloseMainAsync("Already closed main to load colors.  Rethink");
        await CloseMainAsync("");
        ColorScreen = MainContainer.Resolve<RageColorViewModel>();
        await LoadScreenAsync(ColorScreen);
    }
    public async Task CloseColorScreenAsync()
    {
        if (ColorScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(ColorScreen);
        ColorScreen = null;
        await StartNewGameAsync();
    }
    public async Task LoadBidScreenAsync()
    {
        if (BidScreen != null)
        {
            return;
        }
        //await CloseMainAsync("Already closed main to load bidding.  Rethink");
        await CloseMainAsync("");//try to make it ignore if main screen is closed out already.
        BidScreen = MainContainer.Resolve<RageBiddingViewModel>();
        await LoadScreenAsync(BidScreen);
    }
    public async Task CloseBidScreenAsync()
    {
        if (BidScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(BidScreen);
        BidScreen = null;
        await StartNewGameAsync();
    }
    public RageColorViewModel? ColorScreen { get; set; }
    public RageBiddingViewModel? BidScreen { get; set; }
}