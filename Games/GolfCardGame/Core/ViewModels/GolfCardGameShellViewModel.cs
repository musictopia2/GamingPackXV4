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
    public FirstViewModel? FirstScreen { get; set; }
    protected override async Task StartNewGameAsync()
    {
        if (MainVM != null)
        {
            await CloseSpecificChildAsync(MainVM);
            MainVM = null;
        }
        if (FirstScreen != null)
        {
            throw new CustomBasicException("First Screen should be null when loading First Screens");
        }
        FirstScreen = MainContainer.Resolve<FirstViewModel>();
        await LoadScreenAsync(FirstScreen);
    }
    private async Task LoadMainScreenAsync()
    {
        if (FirstScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(FirstScreen);
        FirstScreen = null;
        MainVM = MainContainer.Resolve<GolfCardGameMainViewModel>();
        await LoadScreenAsync(MainVM);
    }
    protected override IMainScreen GetMainViewModel()
    {
        throw new CustomBasicException("Needed to open first screen instead");
    }
}