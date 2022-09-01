namespace PickelCardGame.Core.ViewModels;
public class PickelCardGameShellViewModel : BasicTrickShellViewModel<PickelCardGamePlayerItem>
{
    public PickelCardGameShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        PickelDelegates delegates,
        IToast toast
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        delegates.CloseBiddingAsync = CloseBidAsync;
        delegates.LoadBiddingAsync = LoadBidAsync;
    }
    public PickelBidViewModel? BidScreen { get; set; }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<PickelCardGameMainViewModel>();
        return model;
    }
    private async Task CloseBidAsync()
    {
        if (BidScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(BidScreen);
        BidScreen = null;
        await StartNewGameAsync();
    }
    private async Task LoadBidAsync()
    {
        if (BidScreen != null)
        {
            await CloseSpecificChildAsync(BidScreen);
        }
        BidScreen = MainContainer.Resolve<PickelBidViewModel>();
        await LoadScreenAsync(BidScreen);
    }
    protected override async Task GetStartingScreenAsync()
    {
        await LoadBidAsync();
    }
}