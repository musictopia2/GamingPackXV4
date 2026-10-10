namespace BasicGameFrameworkLibrary.Core.ViewModels;
public abstract partial class SinglePlayerShellViewModel(IGamePackageResolver mainContainer,
    CommandContainer container,
    ISaveSinglePlayerClass saves,
    IEventAggregator aggregator) : IStartShell,
    IHandleAsync<NewGameEventModel>,
    IHandleAsync<GameOverEventModel>,
    IHandle<AskNewGameEventModel>,
    IHandle<AskMainEventModel>,
    IHandle<AskPromptEventModel>,
    IHandle<AskPostGameEventModel>,
    IMainGPXShellVM
{
    //need it for this.
    protected abstract bool AlwaysNewGame { get; }
    /// <summary>
    /// usually can automatically start a new game upon loading.
    /// however some games requires settings to be chosen first.
    /// 
    /// </summary>
    protected virtual bool AutoStartNewGame => true;
    protected virtual bool HasPostGame => false;
    private partial void Subscribe();
    private partial void Unsubscribe();
    private NewGameViewModel? NewGameVM { get; set; }
    private IMainGameVM? MainVM { get; set; }
    private IPostGameVM? PostGameVM { get; set; }
    //this is for single player.
    private IPromptVM? PromptVM { get; set; }

    ///// <summary>
    ///// this is used in cases like mastermind where you need to show a solution when its game over.
    ///// </summary>
    ///// <returns></returns>
    //protected virtual Task HandleGameOverFlowAsync() => Task.CompletedTask;


    

    async Task IStartShell.InitAsync()
    {
        //this was where i had the older activate one.
        Unsubscribe(); //hopefully i can have here just in case.
        Subscribe(); //here is where i have to now subscribe
        if (AlwaysNewGame)
        {
            ShowNewGame();
        }
        if (AutoStartNewGame)
        {
            await StartNewGameAsync();
        }
        else
        {
            await OpenStartingOptionsAsync();
        }
    }
    async Task IHandleAsync<NewGameEventModel>.HandleAsync(NewGameEventModel message)
    {
        if (NewGameVM == null)
        {
            throw new CustomBasicException("New game was not even active.  Therefore, I should not have received message for requesting new game");
        }
        if (AlwaysNewGame == false)
        {
            aggregator.Publish(new CloseNewGameEventModel());
            NewGameVM = null;
        }
        if (MainVM != null)
        {
            await CloseMainViewModelAsync();
        }
        MainVM = null;
        await saves.DeleteSinglePlayerGameAsync();
        await Task.Delay(50);
        await ClosePromptAsync();
        await StartNewGameAsync();
    }
    private async Task ClosePromptAsync()
    {
        await aggregator.PublishAsync(new ClosePromptEventModel());
        aggregator.Publish(new ClosePromptEventModel());
        PromptVM = null;
    }
    private async Task CloseMainViewModelAsync()
    {
        await aggregator.PublishAsync(new CloseMainEventModel());
        aggregator.Publish(new CloseMainEventModel());
    }
    async Task IHandleAsync<GameOverEventModel>.HandleAsync(GameOverEventModel message)
    {
        container.ClearLists();
        if (MainVM == null)
        {
            throw new CustomBasicException("The main view model was not even available.  Rethink");
        }
        await saves.DeleteSinglePlayerGameAsync();

        await CloseMainViewModelAsync();
        MainVM = null;
        if (NewGameVM != null)
        {
            return;
        }
        if (HasPostGame)
        {
            await HandlePostGameAsync();
        }
        if (AutoStartNewGame == false)
        {
            await OpenStartingOptionsAsync();
        }
        else
        {
            ShowNewGame();
        }
    }
    private async Task HandlePostGameAsync()
    {
        PostGameVM = mainContainer.Resolve<IPostGameVM>("");
        PostGameVM.Initialize();
        aggregator.Publish(new OpenPostGameEventModel(PostGameVM));
    }
    protected void ShowNewGame()
    {
        NewGameVM = mainContainer.Resolve<NewGameViewModel>("");
        OpenNewGameUI();
    }
    private void OpenNewGameUI()
    {
        //later will worry about possible memory leak issues.
        if (NewGameVM is null)
        {
            throw new CustomBasicException("Cannot open new game because there was none");
        }
        aggregator.Publish(new OpenNewGameEventModel(NewGameVM));
    }
    private async Task StartNewGameAsync()
    {
        container.ClearLists();
        container.IsExecuting = true;
        container.Processing = true;
        InProgressHelpers.Starting = true;
        MainVM = mainContainer.Resolve<IMainGameVM>();
        MainVM.Initialize(); //i like this way.
        await aggregator.PublishAsync(new OpenMainEventModel(MainVM)); //sends it even though i don't need it.
        aggregator.Publish(new OpenMainEventModel(MainVM));
        bool rets = InProgressHelpers.MoveInProgress;
        InProgressHelpers.MoveInProgress = rets;
        InProgressHelpers.Starting = false;
        FinishInit();
        container.ReportAllCommandStates();
        //container.ReportCommandStateChanged();
    }
    private void FinishInit()
    {
        container.Processing = false;
        container.IsExecuting = false;
    }
    private async Task OpenStartingOptionsAsync()
    {
        //look at what i did for main and repeat for this new item.
        PromptVM = mainContainer.Resolve<IPromptVM>("");
        PromptVM.Initialize(); //i like this way.
        await aggregator.PublishAsync(new OpenPromptEventModel(PromptVM));
        aggregator.Publish(new OpenPromptEventModel(PromptVM));
        FinishOpening();
        FinishInit();
    }
    protected virtual void FinishOpening()
    {
        ShowNewGame(); //i think can allow this (if i am wrong, rethink).
    }
    void IHandle<AskNewGameEventModel>.Handle(AskNewGameEventModel message)
    {
        if (NewGameVM is null)
        {
            return;
        }
        OpenNewGameUI();
    }
    void IHandle<AskMainEventModel>.Handle(AskMainEventModel message)
    {
        if (MainVM is null)
        {
            return;
        }
        aggregator.Publish(new OpenMainEventModel(MainVM));
    }
    void IHandle<AskPromptEventModel>.Handle(AskPromptEventModel message)
    {
        if (PromptVM is null)
        {
            return;
        }
        aggregator.Publish(new OpenPromptEventModel(PromptVM));
    }
    void IHandle<AskPostGameEventModel>.Handle(AskPostGameEventModel message)
    {
        if (PostGameVM is null)
        {
            return;
        }
        aggregator.Publish(new OpenPostGameEventModel(PostGameVM));
    }
}