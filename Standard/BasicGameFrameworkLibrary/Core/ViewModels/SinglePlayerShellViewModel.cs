namespace BasicGameFrameworkLibrary.Core.ViewModels;
public abstract partial class SinglePlayerShellViewModel : ConductorViewModel, IHandleAsync<NewGameEventModel>,
    IHandleAsync<GameOverEventModel>,
    IMainGPXShellVM
{
    private readonly ISaveSinglePlayerClass _saves;
    public SinglePlayerShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        MainContainer = mainContainer; 
        CommandContainer = container;
        GameData = gameData;
        _saves = saves;
    }
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        if (AlwaysNewGame)
        {
            await ShowNewGameAsync();
        }
        if (AutoStartNewGame)
        {
            await StartNewGameAsync();
        }
        else
        {
            await OpenStartingScreensAsync();
        }
    }
    /// <summary>
    /// this should be responsible for any opening screens that is not new game.  new game is automatic.
    /// if we find a case where it can't be automatic, rethink.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OpenStartingScreensAsync() => Task.CompletedTask;
    private async Task StartNewGameAsync()
    {
        CommandContainer.ClearLists();
        MainVM = GetMainViewModel();
        await LoadScreenAsync(MainVM);
        bool rets = InProgressHelpers.MoveInProgress;
        FinishInit();
        InProgressHelpers.MoveInProgress = rets;
    }
    protected void FinishInit()
    {
        CommandContainer.Processing = false;
        CommandContainer.IsExecuting = false;
    }
    protected async Task ShowNewGameAsync()
    {
        NewGameVM = MainContainer.Resolve<INewGameVM>("");
        await LoadScreenAsync(NewGameVM);
    }
    /// <summary>
    /// this is needed because it may need to resolve other things to load other things but not at the beginning.
    /// </summary>
    public IGamePackageResolver MainContainer { get; }
    protected CommandContainer CommandContainer { get; }
    protected IGameInfo GameData { get; }
    public INewGameVM? NewGameVM { get; set; }
    public IMainScreen? MainVM { get; set; } 

    /// <summary>
    /// this is the view model that represents the body.  its used when you decide on new game.
    /// </summary>
    /// <returns></returns>
    protected abstract IMainScreen GetMainViewModel();
    protected abstract bool AlwaysNewGame { get; }
    /// <summary>
    /// usually can automatically start a new game upon loading.
    /// however some games requires settings to be chosen first.
    /// 
    /// </summary>
    protected virtual bool AutoStartNewGame => true;
    async Task IHandleAsync<NewGameEventModel>.HandleAsync(NewGameEventModel message)
    {
        if (NewGameVM == null)
        {
            throw new CustomBasicException("New game was not even active.  Therefore, I should not have received message for requesting new game");
        }
        if (AlwaysNewGame == false)
        {
            await CloseSpecificChildAsync(NewGameVM);
            NewGameVM = null;
        }
        if (MainVM != null)
        {
            await CloseSpecificChildAsync(MainVM);
        }
        MainVM = null;
        await _saves.DeleteSinglePlayerGameAsync();
        await Task.Delay(50);
        await NewGameRequestedAsync();
        await StartNewGameAsync();
    }
    /// <summary>
    /// this is used in cases where somebody actually clicks new game but sometimes extra screens has to be closed out.
    /// i could decide later to have a list it keeps track of to close out.
    /// </summary>
    /// <returns></returns>
    protected virtual Task NewGameRequestedAsync() => Task.CompletedTask;
    /// <summary>
    /// this is used in cases like mastermind where you need to show a solution when its game over.
    /// </summary>
    /// <returns></returns>
    protected virtual Task GameOverScreenAsync() => Task.CompletedTask;
    async Task IHandleAsync<GameOverEventModel>.HandleAsync(GameOverEventModel message)
    {
        CommandContainer.ClearLists();
        if (MainVM == null)
        {
            throw new CustomBasicException("The main view model was not even available.  Rethink");
        }
        await _saves.DeleteSinglePlayerGameAsync();
        await CloseSpecificChildAsync(MainVM);
        MainVM = null;
        if (NewGameVM != null)
        {
            return;
        }
        await GameOverScreenAsync();
        if (AutoStartNewGame == false)
        {
            await OpenStartingScreensAsync();
        }
        else
        {
            await ShowNewGameAsync();
        }
    }
}