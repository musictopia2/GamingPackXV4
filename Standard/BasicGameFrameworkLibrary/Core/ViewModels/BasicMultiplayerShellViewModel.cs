namespace BasicGameFrameworkLibrary.Core.ViewModels;
public abstract partial class BasicMultiplayerShellViewModel<P>(IGamePackageResolver mainContainer,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : IStartShell,
    IHandleAsync<NewGameEventModel>,
    IHandleAsync<GameOverEventModel>,
    IHandle<AskNewGameEventModel>,
    IHandleAsync<NewRoundEventModel>,
    IHandle<AskMainEventModel>,
    IHandle<AskMultiplayerOpeningEventModel<P>>,
    IHandle<AskBeginningColorEventModel>,
    IHandle<AskNewRoundEventModel>,
    IHandle<AskPromptEventModel>,
    IHandleAsync<WaitForHostEventModel>,
    IHandleAsync<StartAutoresumeMultiplayerGameEventModel>,
    IHandleAsync<StartMultiplayerGameEventModel<P>>,
    IHandleAsync<ReconnectEventModel>,
    IHandleAsync<RoundOverEventModel>,
    IHandleAsync<ClosePromptEventModel>,
    IRestoreNM,
    IHandleAsync<RestoreEventModel>,
    INewGameNM,
    ILoadGameNM,
    IMainGPXShellVM
    where P : class, IPlayerItem, new()
{
    private IBasicGameProcesses<P>? _mainGame;
    private partial void Subscribe();
    private partial void Unsubscribe();
    private IMultiplayerOpeningVM<P>? _openingVM;
    private NewGameViewModel? NewGameVM { get; set; }
    private RestoreViewModel? RestoreVM { get; set; }
    private NewRoundViewModel? NewRoundVM { get; set; }
    private IPromptVM? PromptVM { get; set; }
    private IMainGameVM? MainVM { get; set; }
    public string NickName { get; set; } = "";
    protected IGameInfo GameData { get; } = gameData;
    protected BasicData BasicData { get; } = basicData;
    protected bool IsMainOpened => MainVM is not null;


    protected virtual void ClearSubscriptions()
    {
        //still needed because of yahtzee like games.
    }
    async Task IStartShell.InitAsync()
    {
        //this is where everything starts.
        Unsubscribe(); //just in case.
        Subscribe();

        try
        {

            if (OpeningCategory == EnumOpeningCategory.ChooseColor)
            {
                MiscDelegates.ColorsFinishedAsync = CloseColorsAndContinueAsync;
            }
            RegularSimpleCard.ClearSavedList();
            GlobalHelpers.LoadGameScreenAsync = LoadGameScreenAsync;
            if (BasicData.GamePackageMode == EnumGamePackageMode.None)
            {
                throw new CustomBasicException("You need to specify whether its debug or reals");
            }
            if (BasicData.GamePackageMode == EnumGamePackageMode.Production)
            {
                if (test.AllowAnyMove)
                {
                    throw new CustomBasicException("Can't allow any move because its production");
                }
                if (test.SlowerMoves)
                {
                    throw new CustomBasicException("Cannot have much slower moves because its in production");
                }
                if (test.ShowExtraToastsForDebugging)
                {
                    throw new CustomBasicException("Cannot show extra toasts for debugging because its in production");
                }
                if (test.AutoNearEndOfDeckBeginning)
                {
                    throw new CustomBasicException("Can't be near the end of deck at beginning because its production");
                }
                if (test.CardsToPass != 0)
                {
                    throw new CustomBasicException("Cannot pass a special number of cards becuase its production");
                }
                if (test.ComputerEndsTurn)
                {
                    throw new CustomBasicException("The computer cannot just end turn because its production.  Try setting another property");
                }
                if (test.ComputerNoCards)
                {
                    throw new CustomBasicException("The computer has to have cards because its production");
                }
                if (test.DoubleCheck)
                {
                    throw new CustomBasicException("No double checking anything because its production");
                }
                if (test.ImmediatelyEndGame)
                {
                    throw new CustomBasicException("Cannot immediately end game because its production");
                }
                if (test.NoAnimations)
                {
                    throw new CustomBasicException("Animations are required in production.");
                }
                if (test.NoCommonMessages)
                {
                    throw new CustomBasicException("Must have common messages because its production");
                }
                if (test.PlayCategory != EnumTestPlayCategory.Normal)
                {
                    throw new CustomBasicException("The play category must be none because its production");
                }
                if (test.SaveOption != EnumTestSaveCategory.Normal)
                {
                    throw new CustomBasicException("The save mode must be normal because its production");
                }
                if (test.StatePosition != 0)
                {
                    throw new CustomBasicException("Must show most recent state because its in production");
                }
                if (test.ShowErrorMessageBoxes == false)
                {
                    throw new CustomBasicException("Must show error message boxes because its in production");
                }
                if (test.WhoStarts != 1)
                {
                    throw new CustomBasicException("WhoStarts must start with 1 because its in production");
                }
                if (test.ShowNickNameOnShell)
                {
                    throw new CustomBasicException("Cannot show nick name on shell because its in production");
                }
                if (test.AlwaysNewGame)
                {
                    throw new CustomBasicException("Can't always show new game because its in production");
                }
                if (test.EndRoundEarly)
                {
                    throw new CustomBasicException("Can't end round early because its in production");
                }
                if (test.AdvancedTestOptions)
                {
                    throw new CustomBasicException("Can't show advanced test options because its in production");
                }
            }
            //await BeforeLoadingOpeningScreenAsync();
            NickName = BasicData.NickName;
            if (CanStartWithOpen)
            {
                await ShowMultiplayerOpeningOptionsAsync();
            }
        }
        catch (Exception ex)
        {
            toast.ShowInfoToast(ex.Message);
        }
    }

    protected virtual bool CanStartWithOpen => true;

    private async Task ShowMultiplayerOpeningOptionsAsync()
    {
        if (_openingVM is not null)
        {
            return;
        }
        _openingVM = mainContainer.Resolve<IMultiplayerOpeningVM<P>>();
        _openingVM.Initialize();
        if (_openingVM is MultiplayerOpeningViewModel<P> open)
        {
            await aggregator.PublishAsync(new OpenMultiplayerOpeningEventModel<P>(open));
            return;
        }
        OpenMultiplayerOpeningOptions();
    }

    private void OpenMultiplayerOpeningOptions()
    {
        if (_openingVM is null)
        {
            throw new CustomBasicException("There was no opening");
        }
        if (_openingVM is MultiplayerOpeningViewModel<P> open)
        {
            aggregator.Publish(new OpenMultiplayerOpeningEventModel<P>(open));
            return;
        }
        throw new CustomBasicException("For now, i think expects the real one since the open requires it");
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
        ShowMainUI();
    }
    void IHandle<AskMultiplayerOpeningEventModel<P>>.Handle(AskMultiplayerOpeningEventModel<P> message)
    {
        if (_openingVM is null)
        {
            return; //if null, can return at this point.
        }
        OpenMultiplayerOpeningOptions();
    }
    
    async Task IHandleAsync<NewGameEventModel>.HandleAsync(NewGameEventModel message)
    {
        if (test.AlwaysNewGame)
        {
            await save.DeleteGameAsync();
            if (IsPromptOpened)
            {
                ClosePrompt();
            }
            await CloseMainAsync();
            await CloseColorsAloneAsync(); //just in case.

            ReplaceGame();
        }
        IRequestNewGameRound gameRound;
        if (NewGameVM is null)
        {
            if (test.AlwaysNewGame == false)
            {
                await CloseMainAsync();
                if (_mainGame is null)
                {
                    throw new CustomBasicException("Failed to replace game when requesting new game.  Rethink");
                }
                ReplaceGame();
            }
            //not sure if new round or game is needed to be known by child classes.
            

            //await CloseMainAsync("Should have shown main game when showing new game.");
            ////not sure if new round or game is needed to be known by child classes.
            //if (_mainGame == null)
            //{
            //    throw new CustomBasicException("Failed to replace game when requesting new game.  Rethink");
            //}
            
            gameRound = mainContainer.Resolve<IRequestNewGameRound>();
            await gameRound.RequestNewGameAsync();
            return;
        }
        if (NewGameVM is null)
        {
            throw new CustomBasicException("New game was not even active.  Therefore, I should not have received message for requesting new game");
        }
        await CloseMainAsync();
        await save.DeleteGameAsync();
        await CloseNewGameAsync();
        //not sure if somebody needs to handle this or not (?)
        //await NewGameOrRoundRequestedAsync();
        if (_mainGame == null)
        {
            throw new CustomBasicException("Failed to replace game when requesting new game.  Rethink");
        }
        gameRound = mainContainer.Resolve<IRequestNewGameRound>();
        await gameRound.RequestNewGameAsync();
    }

    //not sure if i can make protected or not yet.
    //private async Task CloseMainAsync(string message)
    //{
    //    if (MainVM == null)
    //    {
    //        if (message == "")
    //        {
    //            return;
    //        }
    //        throw new CustomBasicException(message);
    //    }
    //    await CloseMainAsync();
    //    await Task.Delay(50);
    //}
    async Task IHandleAsync<GameOverEventModel>.HandleAsync(GameOverEventModel message)
    {
        ReplaceGame();
        if (BasicData.MultiPlayer == true && BasicData.Client == true)
        {
            return;
        }
        if (MainVM == null)
        {
            throw new CustomBasicException("The main view model was not even available.  Rethink");
        }
        await save.DeleteGameAsync();
        if (MainVM == null)
        {
            throw new CustomBasicException("The main view model was not even available.  Rethink");
        }
        if (test.AlwaysNewGame)
        {
            await CloseNewGameAsync();
        }
        ShowNewGame(); //i think.
    }

    async Task IHandleAsync<WaitForHostEventModel>.HandleAsync(WaitForHostEventModel message)
    {
        await CloseOpenAsync();
    }

    async Task IHandleAsync<StartAutoresumeMultiplayerGameEventModel>.HandleAsync(StartAutoresumeMultiplayerGameEventModel message)
    {
        await CloseOpenAsync();
        IStartMultiPlayerGame<P> starts = mainContainer.Resolve<IStartMultiPlayerGame<P>>();
        await starts.LoadSavedGameAsync();
    }

    async Task IHandleAsync<StartMultiplayerGameEventModel<P>>.HandleAsync(StartMultiplayerGameEventModel<P> message)
    {
        await CloseOpenAsync();
        IStartMultiPlayerGame<P> starts = mainContainer.Resolve<IStartMultiPlayerGame<P>>();
        await starts.LoadNewGameAsync(message.PlayerList);
    }


    async Task IHandleAsync<ReconnectEventModel>.HandleAsync(ReconnectEventModel message)
    {
        IReconnectClientClass client = mainContainer.Resolve<IReconnectClientClass>();
        await client.ReconnectClientAsync(message.NickName);
    }


    async Task ILoadGameNM.LoadGameAsync(string data)
    {
        if (_openingVM is not null)
        {
            throw new CustomBasicException("The opening screen should have been closed.  Rethink");
        }
        ILoadClientGame client = mainContainer.Resolve<ILoadClientGame>();
        await client.LoadGameAsync(data);
    }

    async Task INewGameNM.NewGameReceivedAsync(string data)
    {
        if (_openingVM is not null)
        {
            throw new CustomBasicException("The opening screen should have been closed.  Rethink");
        }
        await RestoreAsync(); //take a risk here.
        IClientUpdateGame clientUpdate = mainContainer.Resolve<IClientUpdateGame>();
        await clientUpdate.UpdateGameAsync(data);
    }
    private async Task RestoreAsync()
    {
        if (NewGameVM is not null)
        {
            await CloseNewGameAsync();
        }
        //later needs rounds.
        ReplaceGame();
        if (MainVM is null)
        {
            return;
        }
        await CloseMainAsync();
    }
    private async Task LoadGameScreenAsync()
    {
        _mainGame ??= mainContainer.Resolve<IBasicGameProcesses<P>>();
        if (_mainGame.CanMakeMainOptionsVisibleAtBeginning)
        {
            await StartNewGameAsync();
            if (test.AlwaysNewGame)
            {
                ShowNewGame();
            }
            return;
        }
        await LoadStartingOptionsAsync();
        //await GetStartingScreenAsync();

        //when i need to choose colors, rethink.  may have to be here since its not just boards games that has to choose colors (sorry card game, sorry diced)

        if (test.AlwaysNewGame)
        {
            ShowNewGame();
        }
    }

    //don't handle this yet.
    //needs to figure out the autoresume route too.
    protected virtual async Task LoadStartingOptionsAsync()
    {
        if (OpeningCategory == EnumOpeningCategory.None)
        {
            //because of rage card game, attempting to do nothing (since something else will force the proper one).
            //throw new CustomBasicException("Must specify starting options since you cannot make main available at beginning");
            return;
        }
        if (OpeningCategory == EnumOpeningCategory.ChooseColor)
        {
            //could be choose color.   could be needs to figure out in real time.
            //may need other (?)
            await OpenChooseColorsAsync();
            return;
        }
        if (OpeningCategory == EnumOpeningCategory.GamePrompt)
        {
            await OpenPromptAsync();
            return;
        }
        throw new CustomBasicException("Not handled yet");
    }
    protected bool IsPromptOpened => PromptVM is not null;
    protected async Task OpenPromptAsync()
    {
        if (PromptVM is not null)
        {
            throw new CustomBasicException("Game prompt is already opened");
        }
        PromptVM = Resolver!.Resolve<IPromptVM>();
        PromptVM.Initialize();
        await aggregator.PublishAsync(new OpenPromptEventModel(PromptVM)); //since its more generic, needs this.  if nobody handles it, no problem
        aggregator.Publish(new OpenPromptEventModel(PromptVM));
    }
    protected void ClosePrompt()
    {
        if (PromptVM is null)
        {
            throw new CustomBasicException("You never had the game prompt opened");
        }
        aggregator.Publish(new ClosePromptEventModel());
        PromptVM = null;
    }
    private async Task OpenChooseColorsAsync()
    {
        BeginningColorVM = mainContainer.Resolve<IBeginningColorViewModel>("");
        BeginningColorVM.Initialize();
        await aggregator.PublishAsync(new OpenBeginningColorEventModel(BeginningColorVM));
        aggregator.Publish(new OpenBeginningColorEventModel(BeginningColorVM));
    }
    private IBeginningColorViewModel? BeginningColorVM { get; set; }
    protected virtual EnumOpeningCategory OpeningCategory
    => EnumOpeningCategory.None;
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
    protected async Task CloseMainAsync()
    {
        if (MainVM is null)
        {
            return;
        }
        await aggregator.PublishAsync(new CloseMainEventModel());
        aggregator.Publish(new CloseMainEventModel());
        MainVM = null;
    }
    private async Task CloseNewGameAsync()
    {
        await aggregator.PublishAsync(new CloseNewGameEventModel());
        aggregator.Publish(new CloseNewGameEventModel());
        NewGameVM = null;
    }
    protected async Task LoadMainAsync()
    {
        if (MainVM is not null)
        {
            await CloseMainAsync();
        }
        MainVM = mainContainer.Resolve<IMainGameVM>();
        MainVM.Initialize();
        await aggregator.PublishAsync(new OpenMainEventModel(MainVM));
        ShowMainUI();
    }
    protected virtual async Task StartNewGameAsync()
    {
        await LoadMainAsync();
        //no problem for single player but not for multiplayer games.
        container.IsExecuting = true;
        container.Processing = true; //both this time.
        //FinishInit();
        if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
        {
            if (RestoreVM is not null)
            {
                await CloseRestore();
            }
            RestoreVM = Resolver!.Resolve<RestoreViewModel>();
            OpenRestoreUI();
        }
    }
    private void OpenRestoreUI()
    {
        if (RestoreVM is null)
        {
            throw new CustomBasicException("Restore was not even opened");
        }
        aggregator.Publish(new OpenRestoreEventModel(RestoreVM));
    }
    private async Task CloseRestore()
    {
        await aggregator.PublishAsync(new CloseRestoreEventModel());
        aggregator.Publish(new CloseRestoreEventModel());
        RestoreVM = null;
    }
    private void ShowMainUI()
    {
        if (MainVM is null)
        {
            throw new CustomBasicException("Main was not even visible");
        }
        aggregator.Publish(new OpenMainEventModel(MainVM));
    }
    protected void FinishInit()
    {
        container.Processing = false;
        container.IsExecuting = false;
    }
    protected virtual void ReplaceVMData()
    {
        _ = mainContainer.ReplaceObject<IViewModelData>();
    }
    protected virtual BasicList<Type> GetAdditionalObjectsToReset() => [];


    protected virtual void ReplaceGame()
    {
        container.ClearLists();

        ClearSubscriptions();
        ReplaceVMData();

        BasicList<Type> thisList = new();
        thisList.AddRange(GetAdditionalObjectsToReset());

        if (MiscDelegates.GetMiscObjectsToReplace != null)
        {
            thisList.AddRange(MiscDelegates.GetMiscObjectsToReplace.Invoke());
        }

        if (MiscDelegates.GetAutoResets != null)
        {
            thisList.AddRange(MiscDelegates.GetAutoResets.Invoke());
        }

        if (MiscDelegates.GetAutoGeneratedObjectsToReplace != null)
        {
            thisList.AddRange(
                MiscDelegates.GetAutoGeneratedObjectsToReplace.Invoke());
        }

        mainContainer.ResetSeveralObjects(thisList);

        // Anything constructed from here forward registers into CLEAN lists.
        _mainGame = mainContainer.ReplaceObject<IBasicGameProcesses<P>>();
    }

    //protected virtual void ReplaceGame()
    //{
    //    ClearSubscriptions();
    //    ReplaceVMData();
    //    BasicList<Type> thisList = new();
    //    thisList.AddRange(GetAdditionalObjectsToReset());
    //    //Type? type = mainContainer.LookUpType<IStandardRollProcesses>();
    //    //if (type != null)
    //    //{
    //    //    thisList.Add(type);
    //    //}
    //    if (MiscDelegates.GetMiscObjectsToReplace != null)
    //    {
    //        thisList.AddRange(MiscDelegates.GetMiscObjectsToReplace.Invoke());
    //    }
    //    if (MiscDelegates.GetAutoResets != null)
    //    {
    //        thisList.AddRange(MiscDelegates.GetAutoResets.Invoke());
    //    }
    //    if (MiscDelegates.GetAutoGeneratedObjectsToReplace != null)
    //    {
    //        thisList.AddRange(MiscDelegates.GetAutoGeneratedObjectsToReplace.Invoke());
    //    }
    //    mainContainer.ResetSeveralObjects(thisList);
    //    _mainGame = mainContainer.ReplaceObject<IBasicGameProcesses<P>>(); //hopefully this works
    //}

    async Task IHandleAsync<RestoreEventModel>.HandleAsync(RestoreEventModel message)
    {
        await RestoreAsync();
        IRestoreMultiPlayerGame restore = mainContainer.Resolve<IRestoreMultiPlayerGame>();
        await restore.RestoreGameAsync();
    }

    private async Task CloseOpenAsync()
    {
        if (_openingVM is null)
        {
            return;
        }
        await aggregator.PublishAsync(new CloseMultiplayerOpeningEventModel<P>());
        aggregator.Publish(new CloseMultiplayerOpeningEventModel<P>());
        _openingVM = null;
    }

    

    void IHandle<AskBeginningColorEventModel>.Handle(AskBeginningColorEventModel message)
    {
        if (BeginningColorVM is null)
        {
            return;
        }
        aggregator.Publish(new OpenBeginningColorEventModel(BeginningColorVM));
    }
    protected virtual bool CanOpenMainAfterColors => true; //so overrided versions can do other things.


    private async Task CloseColorsAndContinueAsync()
    {
        await CloseColorsAloneAsync();
        await AfterCloseColorsAsync();
    }

    private async Task CloseColorsAloneAsync()
    {
        if (BeginningColorVM is not null)
        {
            BeginningColorVM.Close();
            aggregator.Publish(new CloseBeginningColorEventModel());
        }
        
        BeginningColorVM = null;
        
        
    }

    private async Task AfterCloseColorsAsync()
    {
        if (CanOpenMainAfterColors)
        {
            await StartNewGameAsync();
        }
        else
        {
            await OpenPromptAsync();
            //await OpenAdditionalPlayerOptionsAsync();
            //not sure what else is needed.
            //this is where i may need to open misc options (?)
        }
        IAfterColorProcesses processes = mainContainer.Resolve<IAfterColorProcesses>();
        await processes.AfterChoosingColorsAsync();
    }

    
    //protected virtual Task AfterAdditionalPlayerChoicesAsync()
    //{
    //    return Task.CompletedTask;
    //}

    async Task IRestoreNM.RestoreMessageAsync(string payLoad)
    {
        await RestoreAsync();
        IClientUpdateGame client = mainContainer.Resolve<IClientUpdateGame>();
        await client.UpdateGameAsync(payLoad);
    }
    async Task IHandleAsync<NewRoundEventModel>.HandleAsync(NewRoundEventModel message)
    {
        await CloseMainAsync();
        await CloseRoundAsync();
        //hopefully nothing special is needed here anymore (?)
        //await NewGameOrRoundRequestedAsync();
        IRequestNewGameRound gameRound = mainContainer.Resolve<IRequestNewGameRound>();
        await gameRound.RequestNewRoundAsync();
    }
    private async Task CloseRoundAsync()
    {
        if (NewRoundVM is null)
        {
            throw new CustomBasicException("Never had the round screen opened.  Rethink");
        }
        await aggregator.PublishAsync(new CloseNewRoundEventModel());
        aggregator.Publish(new CloseNewRoundEventModel());
        NewRoundVM = null;
    }
    async Task IHandleAsync<RoundOverEventModel>.HandleAsync(RoundOverEventModel message)
    {
        NewRoundVM = mainContainer.Resolve<NewRoundViewModel>();
        await aggregator.PublishAsync(new OpenNewRoundEventModel(NewRoundVM));
        aggregator.Publish(new OpenNewRoundEventModel(NewRoundVM));
        container.ManuelFinish = true;
        container.IsExecuting = true;
    }
    void IHandle<AskNewRoundEventModel>.Handle(AskNewRoundEventModel message)
    {
        if (NewRoundVM is null)
        {
            return;
        }
        aggregator.Publish(new OpenNewRoundEventModel(NewRoundVM));
    }
    

    void IHandle<AskPromptEventModel>.Handle(AskPromptEventModel message)
    {
        if (PromptVM is null)
        {
            return;
        }
        aggregator.Publish(new OpenPromptEventModel(PromptVM));
    }

    protected virtual async Task AfterClosingPromptAsync()
    {
        await StartNewGameAsync();
    }

    async Task IHandleAsync<ClosePromptEventModel>.HandleAsync(ClosePromptEventModel message)
    {
        ClosePrompt();
        await AfterClosingPromptAsync();
    }
}