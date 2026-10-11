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
    }
    private async Task CloseBidAsync()
    {
        if (IsPromptOpened == false)
        {
            return;
        }
        ClosePrompt();
        await LoadMainAsync();
    }
    protected override EnumOpeningCategory OpeningCategory => EnumOpeningCategory.GamePrompt;
}