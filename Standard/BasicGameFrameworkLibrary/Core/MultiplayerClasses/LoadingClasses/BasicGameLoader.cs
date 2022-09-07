namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.LoadingClasses;
public sealed class BasicGameLoader<P, S> : IStartMultiPlayerGame<P>, IClientUpdateGame, ILoadClientGame,
    IRequestNewGameRound, IRestoreMultiPlayerGame, IReconnectClientClass

    where P : class, IPlayerItem, new()
    where S : BasicSavedGameClass<P>, new() //i think the new one was needed after all.
{
    private readonly BasicData _basic;
    private readonly IGameInfo _gameInfo;
    private readonly IMultiplayerSaveState _state;
    private readonly TestOptions _test;
    private IGameSetUp<P, S>? _gameSetUp;
    private readonly IGamePackageResolver _resolver;
    private readonly CommandContainer _command;
    private readonly IEventAggregator _aggregator;
    private readonly IToast _toast;
    private readonly ISystemError _error;
    public BasicGameLoader(BasicData basic,
        IGameInfo gameInfo,
        IMultiplayerSaveState state,
        TestOptions test,
        IGamePackageResolver resolver,
        CommandContainer command,
        IEventAggregator aggregator,
        IToast toast,
        ISystemError error
        )
    {
        _basic = basic;
        _gameInfo = gameInfo;
        _state = state;
        _test = test;
        _resolver = resolver;
        _command = command;
        _aggregator = aggregator;
        _toast = toast;
        _error = error;
    }
    private void SetGame()
    {
        _gameSetUp = _resolver.Resolve<IGameSetUp<P, S>>();
        _gameSetUp.ComputerEndsTurn = _test.ComputerEndsTurn || _gameInfo.PlayerType == EnumPlayerType.NetworkOnly;
        _gameSetUp.FinishUpAsync = FinishUpAsync;
    }
    private async Task FinishUpAsync(bool isBeginning)
    {
        if (GlobalHelpers.LoadGameScreenAsync == null)
        {
            throw new CustomBasicException("Did not set the load game function when finishing up.  Rethink");
        }
        await GlobalHelpers.LoadGameScreenAsync.Invoke();
        await Step1FinishAsync(isBeginning);
        _command.UpdateAll();
        await FinishStartAsync();
    }
    private async Task Step1FinishAsync(bool isBeginning)
    {
        if (_basic.MultiPlayer == true && _basic.Client == false) //i think this has to be here.
        {
            await _gameSetUp!.PopulateSaveRootAsync(); //has to be right before sending it.
            if (isBeginning == true)
            {
                await _gameSetUp.Network!.SendLoadGameMessageAsync(_gameSetUp.SaveRoot!);
            }
            else
            {
                await _gameSetUp.Network!.SendNewGameAsync(_gameSetUp.SaveRoot!); //maybe okay if its new game.
            }
        }
    }
    private async Task StartServerGameAsync()
    {
        if (_basic.MultiPlayer == false || _basic.Client == true)
        {
            return;
        }
        await _gameSetUp!.Network!.StartGameAsync();
    }

    async Task ILoadClientGame.LoadGameAsync(string payLoad)
    {
        SetGame();
        _basic.MultiPlayer = true;
        _basic.Client = true;
        _gameSetUp!.SaveRoot = await js.DeserializeObjectAsync<S>(payLoad);
        _gameSetUp.PlayerList = _gameSetUp.SaveRoot.PlayerList;
        _gameSetUp.PlayerList.FixNetworkedPlayers(_basic.NickName);
        await FinishGetSavedAsync();
        _command.UpdateAll();
        await FinishStartAsync();
        _command.Processing = false;
    }
    private async Task FinishGetSavedAsync()
    {
        _resolver.ReplaceObject(_gameSetUp!.SaveRoot);
        if (_gameSetUp.SaveRoot!.PlayerList != null)
        {
            _gameSetUp.SaveRoot.PlayerList.MainContainer = _resolver;
            _gameSetUp.SaveRoot.PlayerList.AutoSaved(_gameSetUp.SaveRoot.PlayOrder);
            _gameSetUp.PlayerList = _gameSetUp.SaveRoot.PlayerList;
        }
        await _gameSetUp.FinishGetSavedAsync();
        if (GlobalHelpers.LoadGameScreenAsync == null)
        {
            throw new CustomBasicException("Did not set the load game function when getting saved or restored.  Rethink");
        }
        await GlobalHelpers.LoadGameScreenAsync.Invoke();
    }
    private async Task FinishStartAsync()
    {
        bool rets;
        rets = _resolver.RegistrationExist<IFinishStart>();
        if (rets == true)
        {
            IFinishStart thisFinish = _resolver.Resolve<IFinishStart>();
            await thisFinish.FinishStartAsync();
        }
        _gameSetUp!.StartingStatus();
        if (_gameSetUp!.PlayerList?.Count > 0)
        {
            _gameSetUp.SingleInfo = _gameSetUp.PlayerList.GetWhoPlayer();
        }
        _gameSetUp.ShowTurn();
        if (_gameSetUp.SaveRoot!.ImmediatelyStartTurn)
        {
            await _gameSetUp.StartNewTurnAsync();
        }
        else
        {
            await _gameSetUp.ContinueTurnAsync();
        }
    }
    async Task IStartMultiPlayerGame<P>.LoadNewGameAsync(PlayerCollection<P> startList)
    {
        SetGame();
        await StartServerGameAsync();
        _gameSetUp!.SaveRoot!.PlayOrder = (PlayOrderClass)_resolver.Resolve<IPlayOrder>();
        if (_resolver.RegistrationExist<ISetObjects>())
        {
            ISetObjects up = _resolver.Resolve<ISetObjects>();
            await up.SetSaveRootObjectsAsync();
        }
        if (startList.GetTemporaryCount == 0)
        {
            await FinishLoadingAsync(true);
            _command.Processing = false;
            return;
        }
        bool rets;
        if (_test.PlayCategory == EnumTestPlayCategory.Normal)
        {
            rets = true;
        }
        else
        {
            rets = false;
        }
        startList.FinishLoading(rets);
        if (_basic.MultiPlayer == true)
        {
            startList.FixNetworkedPlayers(_basic.NickName);
        }
        _gameSetUp.SaveRoot.PlayOrder.WhoTurn = _test.WhoStarts;
        _gameSetUp.SaveRoot.PlayerList = startList;
        _gameSetUp.PlayerList = startList;
        await FinishLoadingAsync(true);
        _command.Processing = false;
    }
    async Task IStartMultiPlayerGame<P>.LoadSavedGameAsync()
    {
        SetGame();
        await StartServerGameAsync();
        bool rets;
        rets = await GetSavedRootAsync();
        if (rets == false)
        {
            throw new CustomBasicException("You should have loaded new game now");
        }
        await FinishHostSavedAsync();
    }
    private async Task<bool> GetSavedRootAsync()
    {
        string payLoad = await _state.SavedDataAsync<S>();
        if (payLoad == "")
        {
            _gameSetUp!.SaveRoot = _resolver.Resolve<S>();
            return false;
        }
        _gameSetUp!.SaveRoot = await js.DeserializeObjectAsync<S>(payLoad);
        return true;
    }
    private async Task FinishHostSavedAsync()
    {
        await FinishGetSavedAsync();
        if (_basic.MultiPlayer == true)
        {
            await _gameSetUp!.Network!.SendLoadGameMessageAsync(_gameSetUp.SaveRoot!);
        }
        _command.UpdateAll();
        await FinishStartAsync();
        _command.Processing = false;
    }
    async Task IRequestNewGameRound.RequestNewGameAsync()
    {
        SetGame();
        _command.ManuelFinish = true;
        _command.Processing = true;
        if (_gameInfo.GameType == EnumGameType.Rounds)
        {
            IStartNewGame temps = _resolver.Resolve<IStartNewGame>();
            await temps.ResetAsync();
        }
        await FinishLoadingAsync(false);
        _command.Processing = false;
    }
    async Task IRequestNewGameRound.RequestNewRoundAsync()
    {
        if (_gameInfo.GameType == EnumGameType.NewGame)
        {
            throw new CustomBasicException("Rounds was never supported for this game.  Therefore, should have never been allowed");
        }
        _command.ManuelFinish = true;
        await FinishLoadingAsync(false);
        _command.Processing = false;
    }
    private async Task UpdateGameAsync(string data)
    {
        SetGame();
        _gameSetUp!.SaveRoot = await js.DeserializeObjectAsync<S>(data);
        _gameSetUp.PlayerList = _gameSetUp.SaveRoot.PlayerList;
        _gameSetUp.PlayerList.FixNetworkedPlayers(_basic.NickName);
        await FinishGetSavedAsync();
        _command.UpdateAll();
        await FinishStartAsync();
        _command.Processing = false;
    }
    async Task IRestoreMultiPlayerGame.RestoreGameAsync()
    {
        SetGame();
        await GetSavedRootAsync();
        await FinishGetSavedAsync();
        if (_basic.MultiPlayer == true)
        {
            await _gameSetUp!.Network!.SendRestoreGameAsync(_gameSetUp.SaveRoot!);
        }
        _command.UpdateAll();
        await FinishStartAsync();
    }
    async Task IClientUpdateGame.UpdateGameAsync(string payload)
    {
        SetGame();
        await UpdateGameAsync(payload);
    }
    private async Task FinishLoadingAsync(bool isBeginning)
    {
        if (_gameSetUp!.SaveRoot!.PlayerList == null || _gameSetUp.SaveRoot.PlayerList.Count == 0)
        {
            await _gameSetUp.SetUpGameAsync(isBeginning);
            return;
        }
        if (_gameSetUp.SaveRoot.PlayerList.Count == 0 && _gameInfo.SinglePlayerChoice != EnumPlayerChoices.Solitaire)
        {
            throw new CustomBasicException("There was no players.  Rethink");
        }
        if (isBeginning == false)
        {
            _gameSetUp.SaveRoot.PlayerList.ForEach(items => items.InGame = items.CanStartInGame);
            _gameSetUp.SaveRoot.PlayOrder.WhoTurn = _gameSetUp.SaveRoot.PlayOrder.WhoStarts;
            _gameSetUp.SaveRoot.PlayOrder.WhoTurn = await _gameSetUp.SaveRoot.PlayerList.CalculateWhoTurnAsync();
        }
        _gameSetUp.SaveRoot.PlayOrder.WhoStarts = _gameSetUp.SaveRoot.PlayOrder.WhoTurn;
        await _gameSetUp.SetUpGameAsync(isBeginning);
    }
    async Task IReconnectClientClass.ReconnectClientAsync(string nickName)
    {
        if (_basic.MultiPlayer == false)
        {
            _error.ShowSystemError("Only multiplayer games can be reconnected");
            return;
        }
        if (_basic.Client == true)
        {
            _error.ShowSystemError("Clients cannot reconnect anybody");
            return;
        }
        await Task.Delay(500); 
        bool busy = false;
        int x = 0;
        bool sentMessage = false;
        do
        {
            if (InProgressHelpers.MoveInProgress == false)
            {
                break;
            }
            busy = true;
            await Task.Delay(100);
            x++;
            if (x >= 30000 && sentMessage == false)
            {
                sentMessage = true;
                _toast.ShowWarningToast("The Move Is Taking Too Long.  Could be major issue");
            }
        } while (true);
        if (busy)
        {
            _toast.ShowInfoToast("Has to wait 2 seconds to attempt to finish process");
            await Task.Delay(2000);
        }
        _gameSetUp!.Network!.ClearMessages();
        if (_gameInfo.CanAutoSave == false)
        {
            _toast.ShowInfoToast($"Has to start new game because {nickName} was reconnected but the game does not support autoresume");
            await _aggregator.PublishAsync(new EndGameEarlyEventModel());
            await _gameSetUp!.Network!.EndGameEarlyAsync(nickName);
            await _aggregator.PublishAsync(new NewGameEventModel());
            return;
        }
        _toast.ShowInfoToast($"{nickName} was reconnected");
        await _aggregator.PublishAsync(new RestoreEventModel());
    }
}