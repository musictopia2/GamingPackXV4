namespace BasicGameFrameworkLibrary.Core.ViewModels;

public abstract class BasicBoardGamesShellViewModel<P> : BasicMultiplayerShellViewModel<P>, IBasicBoardGamesShellViewModel where P : class, IPlayerColors, new()
{
    public BasicBoardGamesShellViewModel(
        IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        IToast toast
        ) : base(mainContainer,
            container,
            gameData,
            basicData,
            save,
            test,
            aggregator,
            toast
            )
    {
        MiscDelegates.ColorsFinishedAsync = CloseColorsAsync; //hopefully this simple this time.
    }
    public IBeginningColorViewModel? ColorScreen { get; set; }
    protected override async Task GetStartingScreenAsync()
    {
        if (ColorScreen != null)
        {
            await CloseSpecificChildAsync(ColorScreen);
        }
        ColorScreen = MainContainer.Resolve<IBeginningColorViewModel>();
        await LoadScreenAsync(ColorScreen);
    }
    protected virtual bool CanOpenMainAfterColors => true; //so overrided versions can do other things.
    private async Task CloseColorsAsync()
    {
        if (ColorScreen == null)
        {
            throw new CustomBasicException("The color screen was not even active.  Rethink");
        }
        await CloseSpecificChildAsync(ColorScreen);
        ColorScreen = null;
        if (CanOpenMainAfterColors)
        {
            await StartNewGameAsync();
        }
        IAfterColorProcesses processes = MainContainer.Resolve<IAfterColorProcesses>();
        await processes.AfterChoosingColorsAsync(); //hopefully this simple.
    }
}