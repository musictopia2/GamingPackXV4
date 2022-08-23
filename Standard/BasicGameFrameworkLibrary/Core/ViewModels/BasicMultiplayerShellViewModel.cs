namespace BasicGameFrameworkLibrary.Core.ViewModels;

public abstract partial class BasicMultiplayerShellViewModel<P> : ConductorViewModel,
    IHandleAsync<NewGameEventModel>,
    IHandleAsync<GameOverEventModel>,
    IHandleAsync<NewRoundEventModel>,
    IHandleAsync<WaitForHostEventModel>,
    IHandleAsync<StartAutoresumeMultiplayerGameEventModel>,
    IHandleAsync<StartMultiplayerGameEventModel<P>>,
    IHandleAsync<RoundOverEventModel>,
    IHandleAsync<ReconnectEventModel>,
    INewGameNM,
    ILoadGameNM,
    IMainGPXShellVM,
    IHandleAsync<LoadMainScreenEventModel>,
    IRestoreNM,
    IHandleAsync<RestoreEventModel>,
    IBasicMultiplayerShellViewModel
    where P : class, IPlayerItem, new()
{
    public BasicMultiplayerShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        IToast toast
        ) : base(aggregator)
    {
        MainContainer = mainContainer; //the subscribe part is already done for me too.
        CommandContainer = container;
        GameData = gameData;
        BasicData = basicData;
        _save = save;
        _test = test;
        _toast = toast;
    }
    public string NickName { get; set; } = ""; //if you need nick name shown for test purposes that is an option.
    public IGamePackageResolver MainContainer { get; }
    protected CommandContainer CommandContainer { get; }
    protected IGameInfo GameData { get; }
    protected BasicData BasicData { get; }
    protected override async Task ActivateAsync()
    {
        try
        {
            RegularSimpleCard.ClearSavedList();
            GlobalHelpers.LoadGameScreenAsync = LoadGameScreenAsync;
            if (BasicData.GamePackageMode == EnumGamePackageMode.None)
            {
                throw new CustomBasicException("You need to specify whether its debug or reals");
            }
            if (BasicData.GamePackageMode == EnumGamePackageMode.Production)
            {
                if (_test.AllowAnyMove)
                {
                    throw new CustomBasicException("Can't allow any move because its production");
                }
                if (_test.SlowerMoves)
                {
                    throw new CustomBasicException("Cannot have much slower moves because its in production");
                }
                if (_test.ShowExtraToastsForDebugging)
                {
                    throw new CustomBasicException("Cannot show extra toasts for debugging because its in production");
                }
                if (_test.AutoNearEndOfDeckBeginning)
                {
                    throw new CustomBasicException("Can't be near the end of deck at beginning because its production");
                }
                if (_test.CardsToPass != 0)
                {
                    throw new CustomBasicException("Cannot pass a special number of cards becuase its production");
                }
                if (_test.ComputerEndsTurn)
                {
                    throw new CustomBasicException("The computer cannot just end turn because its production.  Try setting another property");
                }
                if (_test.ComputerNoCards)
                {
                    throw new CustomBasicException("The computer has to have cards because its production");
                }
                if (_test.DoubleCheck)
                {
                    throw new CustomBasicException("No double checking anything because its production");
                }
                if (_test.ImmediatelyEndGame)
                {
                    throw new CustomBasicException("Cannot immediately end game because its production");
                }
                if (_test.NoAnimations)
                {
                    throw new CustomBasicException("Animations are required in production.");
                }
                if (_test.NoCommonMessages)
                {
                    throw new CustomBasicException("Must have common messages because its production");
                }
                if (_test.PlayCategory != EnumTestPlayCategory.Normal)
                {
                    throw new CustomBasicException("The play category must be none because its production");
                }
                if (_test.SaveOption != EnumTestSaveCategory.Normal)
                {
                    throw new CustomBasicException("The save mode must be normal because its production");
                }
                if (_test.StatePosition != 0)
                {
                    throw new CustomBasicException("Must show most recent state because its in production");
                }
                if (_test.ShowErrorMessageBoxes == false)
                {
                    throw new CustomBasicException("Must show error message boxes because its in production");
                }
                if (_test.WhoStarts != 1)
                {
                    throw new CustomBasicException("WhoStarts must start with 1 because its in production");
                }
                if (_test.ShowNickNameOnShell)
                {
                    throw new CustomBasicException("Cannot show nick name on shell because its in production");
                }
                if (_test.AlwaysNewGame)
                {
                    throw new CustomBasicException("Can't always show new game because its in production");
                }
                if (_test.EndRoundEarly)
                {
                    throw new CustomBasicException("Can't end round early because its in production");
                }
            }
            await BeforeLoadingOpeningScreenAsync();
            NickName = BasicData.NickName; //i think.
            if (CanStartWithOpenScreen)
            {
                OpeningScreen = MainContainer.Resolve<IMultiplayerOpeningViewModel>();
                await LoadScreenAsync(OpeningScreen); //so for testing purposes like three letter fun or other situations, i can more easily test.
            }
        }
        catch (Exception ex)
        {
            _toast.ShowInfoToast(ex.Message);
        }

    }
    protected virtual bool CanStartWithOpenScreen => true;
    protected virtual Task BeforeLoadingOpeningScreenAsync() => Task.CompletedTask;
    public IMultiplayerOpeningViewModel? OpeningScreen { get; set; } //has to be property so it hooks up properly.
    public INewRoundVM? NewRoundScreen { get; set; }
    public INewGameVM? NewGameScreen { get; set; }
    private readonly IMultiplayerSaveState _save;
    private readonly TestOptions _test;
    private readonly IToast _toast;
    private IBasicGameProcesses<P>? _mainGame;
    public IMainScreen? MainVM { get; set; } //this is another one for the ui to know.
    protected virtual Task NewGameOrRoundRequestedAsync() => Task.CompletedTask;
    /// <summary>
    /// this is when somebody chooses new game.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    async Task IHandleAsync<NewGameEventModel>.HandleAsync(NewGameEventModel message)
    {
        if (_test.AlwaysNewGame)
        {
            CommandContainer.ClearLists(); //try this too.
            await _save.DeleteGameAsync(); //i think.
            ReplaceGame(); //try this too.
        }
        IRequestNewGameRound gameRound;
        if (NewGameScreen is null)
        {
            await CloseMainAsync("Should have shown main game when showing new game.");
            await NewGameOrRoundRequestedAsync();
            if (_mainGame == null)
            {
                throw new CustomBasicException("Failed to replace game when requesting new game.  Rethink");
            }
            ReplaceGame(); //try this too.
            gameRound = MainContainer.Resolve<IRequestNewGameRound>();
            await gameRound.RequestNewGameAsync();
            return;
        }

        if (NewGameScreen == null)
        {
            throw new CustomBasicException("New game was not even active.  Therefore, I should not have received message for requesting new game");
        }
        await CloseMainAsync("Should have shown main game when showing new game.");
        await _save.DeleteGameAsync(); //i think.
        await CloseSpecificChildAsync(NewGameScreen);
        NewGameScreen = null;//forgot to set to null.
        await NewGameOrRoundRequestedAsync();
        if (_mainGame == null)
        {
            throw new CustomBasicException("Failed to replace game when requesting new game.  Rethink");
        }
        gameRound = MainContainer.Resolve<IRequestNewGameRound>();
        await gameRound.RequestNewGameAsync();
    }
    protected async Task CloseMainAsync(string message)
    {
        if (MainVM == null)
        {
            if (message == "")
            {
                return;
            }
            throw new CustomBasicException(message);
        }
        await CloseSpecificChildAsync(MainVM!);
        MainVM = null; //looks like i have to set to null manually.
        await Task.Delay(50); //try to set here just in case.
    }
    private async Task LoadGameScreenAsync()
    {
        if (_mainGame == null)
        {
            _mainGame = MainContainer.Resolve<IBasicGameProcesses<P>>();
        }
        if (_mainGame.CanMakeMainOptionsVisibleAtBeginning)
        {
            await StartNewGameAsync();
            if (_test.AlwaysNewGame)
            {
                NewGameScreen = MainContainer.Resolve<INewGameVM>();
                await LoadScreenAsync(NewGameScreen);
            }
            return;
        }
        await GetStartingScreenAsync();
        if (_test.AlwaysNewGame)
        {
            NewGameScreen = MainContainer.Resolve<INewGameVM>();
            await LoadScreenAsync(NewGameScreen);
        }
    }

    /// <summary>
    /// this is used for cases like when you have to choose colors.  good example are the board games.
    /// </summary>
    /// <returns></returns>
    protected async virtual Task GetStartingScreenAsync() => await Task.CompletedTask;

    /// <summary>
    /// this is the view model that represents the body.  its used when you decide on new game.
    /// </summary>
    /// <returns></returns>
    protected abstract IMainScreen GetMainViewModel();
    protected virtual async Task StartNewGameAsync()
    {
        if (MainVM != null)
        {
            await CloseSpecificChildAsync(MainVM);
            MainVM = null;
        }
        MainVM = GetMainViewModel();
        await LoadScreenAsync(MainVM);
    }
    protected virtual async Task ShowNewGameAsync()
    {
        NewGameScreen = MainContainer.Resolve<INewGameVM>();
        await LoadScreenAsync(NewGameScreen);
        CommandContainer.IsExecuting = true;
        CommandContainer.ManuelFinish = true;
    }

    /// <summary>
    /// this is when the game is over.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    async Task IHandleAsync<GameOverEventModel>.HandleAsync(GameOverEventModel message) //done.
    {

        //i propose just having the extra button for new game that appears when the game is over.
        CommandContainer.ClearLists(); //try this too.
        ReplaceGame(); //i think here it should replace the game. not so for rounds.
                       //replacegame is where the problem is at.  for clients, that seems to happen as well.
        if (BasicData.MultiPlayer == true && BasicData.Client == true)
        {
            return; //because only host can choose new game unless its single player game.
        }
        await _save.DeleteGameAsync();
        if (MainVM == null)
        {
            throw new CustomBasicException("The main view model was not even available.  Rethink");
        }
        if (_test.AlwaysNewGame)
        {
            await CloseSpecificChildAsync(NewGameScreen!);
            NewGameScreen = null; //has to do again.  that is the best way to handle so i can click new game.
        }
        await ShowNewGameAsync();
    }
    protected virtual void ReplaceVMData()
    {
        _ = MainContainer.ReplaceObject<IViewModelData>(); //this has to be replaced before the game obviously.
    }
    protected virtual void ClearSubscriptions()
    {
        Aggregator.Clear<AskEventModel>(); //trying this one too.
    }
    protected virtual void ReplaceGame()
    {
        ClearSubscriptions();
        ReplaceVMData();
        //Assembly assembly = Assembly.GetAssembly(GetType())!;
        //BasicList<Type> thisList = assembly.GetTypes().Where(items => items.HasAttribute<AutoResetAttribute>()).ToBasicList();
        BasicList<Type> thisList = new();
        thisList.AddRange(GetAdditionalObjectsToReset());
        Type? type = MainContainer.LookUpType<IStandardRollProcesses>();
        if (type != null)
        {
            thisList.Add(type);
        }
        if (MiscDelegates.GetMiscObjectsToReplace != null)
        {
            thisList.AddRange(MiscDelegates.GetMiscObjectsToReplace.Invoke());
        }
        if (MiscDelegates.GetAutoResets != null)
        {
            thisList.AddRange(MiscDelegates.GetAutoResets.Invoke()); //this way 2 source generations can do their jobs.
        }
        if (MiscDelegates.GetAutoGeneratedObjectsToReplace != null)
        {
            thisList.AddRange(MiscDelegates.GetAutoGeneratedObjectsToReplace.Invoke());
        }
        //since i don't have the autoreset attribute, has to later figure out what to do about that part now (?)
        //probably would use a source generator (so no reflection).
        MainContainer.ResetSeveralObjects(thisList);
        _mainGame = MainContainer.ReplaceObject<IBasicGameProcesses<P>>(); //hopefully this works
    }
    protected virtual BasicList<Type> GetAdditionalObjectsToReset() => new();
    private async Task CloseOpenScreenAsync(string message)
    {
        if (OpeningScreen == null)
        {
            if (message == "")
            {
                return;//ignore if its already null.
            }
            throw new CustomBasicException(message);
        }
        await CloseSpecificChildAsync(OpeningScreen);
        OpeningScreen = null;
    }
    async Task IHandleAsync<WaitForHostEventModel>.HandleAsync(WaitForHostEventModel message)
    {
        await CloseOpenScreenAsync("");
    }
    async Task IHandleAsync<StartAutoresumeMultiplayerGameEventModel>.HandleAsync(StartAutoresumeMultiplayerGameEventModel message)
    {
        await CloseOpenScreenAsync("Start autoresume multiplayer game");
        IStartMultiPlayerGame<P> starts = MainContainer.Resolve<IStartMultiPlayerGame<P>>();
        await starts.LoadSavedGameAsync();
    }
    async Task IHandleAsync<StartMultiplayerGameEventModel<P>>.HandleAsync(StartMultiplayerGameEventModel<P> message)
    {
        await CloseOpenScreenAsync("Start new multiplayer game");
        IStartMultiPlayerGame<P> starts = MainContainer.Resolve<IStartMultiPlayerGame<P>>();
        await starts.LoadNewGameAsync(message.PlayerList);
    }
    async Task IHandleAsync<RoundOverEventModel>.HandleAsync(RoundOverEventModel message)
    {

        NewRoundScreen = MainContainer.Resolve<INewRoundVM>();
        await LoadScreenAsync(NewRoundScreen);
        CommandContainer.ManuelFinish = true;
        CommandContainer.IsExecuting = true;
    }
    private async Task CloseRoundAsync()
    {
        if (NewRoundScreen == null)
        {
            throw new CustomBasicException("Never had the round screen opened.  Rethink");
        }
        await CloseSpecificChildAsync(NewRoundScreen);
        NewRoundScreen = null;
    }
    async Task IHandleAsync<NewRoundEventModel>.HandleAsync(NewRoundEventModel message)
    {
        await CloseMainAsync("The main screen should have been not null when choosing new round.  Rethink");
        await CloseRoundAsync();
        await NewGameOrRoundRequestedAsync();
        IRequestNewGameRound gameRound = MainContainer.Resolve<IRequestNewGameRound>();
        await gameRound.RequestNewRoundAsync();
    }
    async Task INewGameNM.NewGameReceivedAsync(string data)
    {
        if (OpeningScreen != null)
        {
            throw new CustomBasicException("The opening screen should have been closed.  Rethink");
        }
        await RestoreAsync(); //take a risk here.
        IClientUpdateGame clientUpdate = MainContainer.Resolve<IClientUpdateGame>();
        await clientUpdate.UpdateGameAsync(data);
    }
    async Task ILoadGameNM.LoadGameAsync(string data) //first time only.
    {
        if (OpeningScreen != null)
        {
            throw new CustomBasicException("The opening screen should have been closed.  Rethink");
        }
        //trying to ignore the mainview model already being opened because of rejoining.
        //if (MainVM != null)
        //{
        //    throw new CustomBasicException("The main screen was already opened.  Rethink");
        //}
        ILoadClientGame client = MainContainer.Resolve<ILoadClientGame>();
        await client.LoadGameAsync(data);
    }
    async Task IHandleAsync<LoadMainScreenEventModel>.HandleAsync(LoadMainScreenEventModel message)
    {
        await StartNewGameAsync(); //i think this simple now.
    }
    async Task IRestoreNM.RestoreMessageAsync(string payLoad)
    {
        await RestoreAsync();
        IClientUpdateGame client = MainContainer.Resolve<IClientUpdateGame>();
        await client.UpdateGameAsync(payLoad);
    }
    private async Task RestoreAsync()
    {
        if (NewRoundScreen != null)
        {
            await CloseSpecificChildAsync(NewRoundScreen);
            NewRoundScreen = null;
        }
        else if (NewGameScreen != null)
        {
            await CloseSpecificChildAsync(NewGameScreen);
            NewGameScreen = null;
        }
        ReplaceGame();
        if (MainVM is null)
        {
            return; //to attempt to make it simple.
        }
        await CloseMainAsync("Failed to restore because never had game");
    }
    async Task IHandleAsync<RestoreEventModel>.HandleAsync(RestoreEventModel message)
    {
        await RestoreAsync();
        IRestoreMultiPlayerGame restore = MainContainer.Resolve<IRestoreMultiPlayerGame>();
        await restore.RestoreGameAsync();
    }

    async Task IHandleAsync<ReconnectEventModel>.HandleAsync(ReconnectEventModel message)
    {
        //hopefully the game loader can figure out what is needed.
        //if not, then rethinking is required.
        IReconnectClientClass client = MainContainer.Resolve<IReconnectClientClass>();
        await client.ReconnectClientAsync(message.NickName);
    }
}