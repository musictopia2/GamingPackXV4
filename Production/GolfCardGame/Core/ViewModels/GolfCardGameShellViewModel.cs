namespace GolfCardGame.Core.ViewModels;
public class GolfCardGameShellViewModel : BasicMultiplayerShellViewModel<GolfCardGamePlayerItem>
{
    public GolfCardGameShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        GolfDelegates delegates,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        delegates.LoadMainScreenAsync = LoadMainScreenAsync;
    }
    protected override async Task StartNewGameAsync()
    {
        if (IsPromptOpened)
        {
            throw new CustomBasicException("First Screen should be null when loading First Screens");
        }
        await CloseMainAsync();
        await OpenPromptAsync();
    }
    private async Task LoadMainScreenAsync()
    {
        if (IsPromptOpened == false)
        {
            return; //hopefully this is play.
        }
        ClosePrompt();
        await base.StartNewGameAsync();
    }   
}