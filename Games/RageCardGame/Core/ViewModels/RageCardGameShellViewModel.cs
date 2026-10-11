namespace RageCardGame.Core.ViewModels;
public class RageCardGameShellViewModel : BasicTrickShellViewModel<RageCardGamePlayerItem>
{
    private EnumScreenCategory _category = EnumScreenCategory.None;
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
        delgates.GetCategory = GetCategory;
    }
    private EnumScreenCategory GetCategory() => _category;
    private async Task LoadColorScreenAsync()
    {
        if (_category == EnumScreenCategory.Color)
        {
            return;
        }
        await CloseMainAsync();
        _category = EnumScreenCategory.Color;
        await OpenPromptAsync();
    }
    public async Task CloseColorScreenAsync()
    {
        if (_category != EnumScreenCategory.Color)
        {
            return;
        }
        ClosePrompt();
        _category = EnumScreenCategory.None;
        await LoadMainAsync();
    }
    private async Task LoadBidScreenAsync()
    {
        if (_category == EnumScreenCategory.Bid)
        {
            return;
        }
        await CloseMainAsync();
        _category = EnumScreenCategory.Bid;
        await OpenPromptAsync();
    }
    private async Task CloseBidScreenAsync()
    {
        if (_category != EnumScreenCategory.Bid)
        {
            return;
        }
        ClosePrompt();
        _category = EnumScreenCategory.None;

        await LoadMainAsync();
    }
}