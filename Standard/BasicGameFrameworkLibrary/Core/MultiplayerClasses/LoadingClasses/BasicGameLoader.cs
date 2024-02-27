namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.LoadingClasses;
public sealed class BasicGameLoader<P, S>(BasicData basic,
    IGameInfo gameInfo,
    IMultiplayerSaveState state,
    TestOptions test,
    IGamePackageResolver resolver,
    CommandContainer command,
    IEventAggregator aggregator,
    IToast toast,
    ISystemError error
        ) : IStartMultiPlayerGame<P>, IClientUpdateGame, ILoadClientGame,
    IRequestNewGameRound, IRestoreMultiPlayerGame, IReconnectClientClass

    where P : class, IPlayerItem, new()
    where S : BasicSavedGameClass<P>, new() //i think the new one was needed after all.
{
    private IGameSetUp<P, S>? _gameSetUp;
    private void SetGame()
    {
        _gameSetUp = resolver.Resolve<IGameSetUp<P, S>>();
        _gameSetUp.ComputerEndsTurn = test.ComputerEndsTurn || gameInfo.PlayerType == EnumPlayerType.NetworkOnly;
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
        command.UpdateAll();
        await FinishStartAsync();
    }
    private async Task Step1FinishAsync(bool isBeginning)
    {
        if (basic.MultiPlayer == true && basic.Client == false) //i think this has to be here.
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
        if (basic.MultiPlayer == false || basic.Client == true)
        {
            return;
        }
        await _gameSetUp!.Network!.StartGameAsync();
    }
    async Task ILoadClientGame.LoadGameAsync(string payLoad)
    {
        SetGame();
        basic.MultiPlayer = true;
        basic.Client = true;
        _gameSetUp!.SaveRoot = await js1.DeserializeObjectAsync<S>(payLoad);
        _gameSetUp.PlayerList = _gameSetUp.SaveRoot.PlayerList;
        _gameSetUp.PlayerList.FixNetworkedPlayers(basic.NickName);
        await FinishGetSavedAsync();
        command.UpdateAll();
        await FinishStartAsync();
        command.Processing = false;
    }
    private async Task FinishGetSavedAsync()
    {
        resolver.ReplaceObject(_gameSetUp!.SaveRoot);
        if (_gameSetUp.SaveRoot!.PlayerList != null)
        {
            _gameSetUp.SaveRoot.PlayerList.MainContainer = resolver;
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
        rets = resolver.RegistrationExist<IFinishStart>();
        if (rets == true)
        {
            IFinishStart thisFinish = resolver.Resolve<IFinishStart>();
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
        _gameSetUp!.SaveRoot!.PlayOrder = (PlayOrderClass)resolver.Resolve<IPlayOrder>();
        if (resolver.RegistrationExist<ISetObjects>())
        {
            ISetObjects up = resolver.Resolve<ISetObjects>();
            await up.SetSaveRootObjectsAsync();
        }
        if (startList.GetTemporaryCount == 0)
        {
            await FinishLoadingAsync(true);
            command.Processing = false;
            return;
        }
        bool rets;
        if (test.PlayCategory == EnumTestPlayCategory.Normal)
        {
            rets = true;
        }
        else
        {
            rets = false;
        }
        if (NewGameContainer.NewGameHost is not null)
        {
            rets = false; //can't shuffle because new game.
        }
        startList.FinishLoading(rets);
        if (basic.MultiPlayer == true)
        {
            startList.FixNetworkedPlayers(basic.NickName);
        }
        if (NewGameContainer.NewGameHost is not null)
        {
            _gameSetUp.SaveRoot.PlayOrder.WhoStarts = NewGameContainer.NewGameHost.WhoStarts;
            _gameSetUp.SaveRoot.PlayOrder.WhoTurn = NewGameContainer.NewGameHost.WhoStarts;
        }
        else
        {
            _gameSetUp.SaveRoot.PlayOrder.WhoTurn = test.WhoStarts;
        }
        _gameSetUp.SaveRoot.PlayerList = startList;
        _gameSetUp.PlayerList = startList;
        await FinishLoadingAsync(true);
        command.Processing = false;
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
        GlobalDelegates.AddNewGame?.Invoke(_gameSetUp!.SaveRoot.GameID); //i think here needs to be done.
        await FinishHostSavedAsync();
    }
    private async Task<bool> GetSavedRootAsync()
    {
        string payLoad = await state.SavedDataAsync<S>();
        if (payLoad == "")
        {
            _gameSetUp!.SaveRoot = resolver.Resolve<S>();
            return false;
        }
        _gameSetUp!.SaveRoot = await js1.DeserializeObjectAsync<S>(payLoad);
        return true;
    }
    private async Task FinishHostSavedAsync()
    {
        await FinishGetSavedAsync();
        if (basic.MultiPlayer == true)
        {
            await _gameSetUp!.Network!.SendLoadGameMessageAsync(_gameSetUp.SaveRoot!);
        }
        command.UpdateAll();
        await FinishStartAsync();
        command.Processing = false;
    }
    async Task IRequestNewGameRound.RequestNewGameAsync()
    {
        SetGame();
        command.ManuelFinish = true;
        command.Processing = true;
        if (gameInfo.GameType == EnumGameType.Rounds)
        {
            IStartNewGame temps = resolver.Resolve<IStartNewGame>();
            await temps.ResetAsync();
        }
        if (gameInfo.SinglePlayerChoice == EnumPlayerChoices.Solitaire)
        {
            await FinishLoadingAsync(false);
            command.Processing = false;
            return;
        }
        foreach (var item in _gameSetUp!.PlayerList)
        {
            item.InGame = true;
        }
        _gameSetUp!.SaveRoot.PlayOrder.IsReversed = false; //can be marked to false now because its a brand new game.
        _gameSetUp.SaveRoot.PlayOrder.WhoTurn = _gameSetUp.SaveRoot.PlayOrder.WhoStarts; //this is like before for who starts again for new game
        _gameSetUp.SaveRoot.PlayOrder.WhoTurn = await _gameSetUp.SaveRoot.PlayerList.CalculateWhoTurnAsync();
        _gameSetUp.SaveRoot.PlayOrder.WhoStarts = _gameSetUp.SaveRoot.PlayOrder.WhoTurn;
        RawGameHost data = new();
        data.WhoStarts = _gameSetUp.SaveRoot.PlayOrder.WhoStarts;
        if (data.WhoStarts == 0)
        {
            toast!.ShowUserErrorToast("WhoStarts cannot be 0.  This means new game will not work");
            return;
        }
        data.GameName = gameInfo.GameName;
        data.Multiplayer = basic.MultiPlayer; //i think
        foreach (var item in _gameSetUp.PlayerList)
        {
            RawPlayer player = new()
            {
                Id = item.Id,
                NickName = item.NickName,
                PlayerCategory = item.PlayerCategory,
                IsHost = item.IsHost,
            };
            data.Players.Add(player);
        }
        if (NewGameDelegates.NewGameHostStep1 is null)
        {
            toast.ShowInfoToast("So far, nobody is handling new game.  Means its stuck for now");
            return;
        }
        await NewGameDelegates.NewGameHostStep1?.Invoke(data)!;
    }
    async Task IRequestNewGameRound.RequestNewRoundAsync()
    {
        if (gameInfo.GameType == EnumGameType.NewGame)
        {
            throw new CustomBasicException("Rounds was never supported for this game.  Therefore, should have never been allowed");
        }
        command.ManuelFinish = true;
        await FinishLoadingAsync(false);
        command.Processing = false;
    }
    private async Task UpdateGameAsync(string data)
    {
        SetGame();
        _gameSetUp!.SaveRoot = await js1.DeserializeObjectAsync<S>(data);
        _gameSetUp.PlayerList = _gameSetUp.SaveRoot.PlayerList;
        _gameSetUp.PlayerList.FixNetworkedPlayers(basic.NickName);
        //this means needs to remove all the private stuff except for the key used.
        //the host will never do anything here.
        if (GlobalDelegates.DeleteOldPrivateGames is not null)
        {
            //this means if something sets it, then use it.
            await GlobalDelegates.DeleteOldPrivateGames(_gameSetUp.SaveRoot.GameID); //delete anything that is not the key.
        }
        await FinishGetSavedAsync();
        command.UpdateAll();
        await FinishStartAsync();
        command.Processing = false;
    }
    async Task IRestoreMultiPlayerGame.RestoreGameAsync()
    {
        SetGame();

        await GetSavedRootAsync();
        await FinishGetSavedAsync();
        if (basic.MultiPlayer == true)
        {
            await _gameSetUp!.Network!.SendRestoreGameAsync(_gameSetUp.SaveRoot!);
        }
        command.UpdateAll();
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
        if (_gameSetUp.SaveRoot.PlayerList.Count == 0 && gameInfo.SinglePlayerChoice != EnumPlayerChoices.Solitaire)
        {
            throw new CustomBasicException("There was no players.  Rethink");
        }
        if (isBeginning == false)
        {
            if (basic.MultiPlayer)
            {
                await _gameSetUp.Network!.SendAllAsync("newround", _gameSetUp.SaveRoot.GameID);
            }
            if (GlobalDelegates.DeleteOldPrivateGames is not null)
            {
                await GlobalDelegates.DeleteOldPrivateGames.Invoke(_gameSetUp.SaveRoot.GameID);
            }
            _gameSetUp.SaveRoot.PlayerList.ForEach(items => items.InGame = items.CanStartInGame);
            _gameSetUp.SaveRoot.PlayOrder.WhoTurn = _gameSetUp.SaveRoot.PlayOrder.WhoStarts;
            _gameSetUp.SaveRoot.PlayOrder.WhoTurn = await _gameSetUp.SaveRoot.PlayerList.CalculateWhoTurnAsync();
        }
        else
        {
            _gameSetUp.SaveRoot.GetNewID();
            GlobalDelegates.AddNewGame?.Invoke(_gameSetUp.SaveRoot.GameID);

        }
        _gameSetUp.SaveRoot.PlayOrder.WhoStarts = _gameSetUp.SaveRoot.PlayOrder.WhoTurn;
        await _gameSetUp.SetUpGameAsync(isBeginning);
    }
    async Task IReconnectClientClass.ReconnectClientAsync(string nickName)
    {
        if (basic.MultiPlayer == false)
        {
            error.ShowSystemError("Only multiplayer games can be reconnected");
            return;
        }
        if (basic.Client == true)
        {
            error.ShowSystemError("Clients cannot reconnect anybody");
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
                toast.ShowWarningToast("The Move Is Taking Too Long.  Could be major issue");
            }
        } while (true);
        if (busy)
        {
            toast.ShowInfoToast("Has to wait 2 seconds to attempt to finish process");
            await Task.Delay(2000);
        }
        _gameSetUp!.Network!.ClearMessages();
        if (gameInfo.CanAutoSave == false)
        {
            toast.ShowInfoToast($"Has to start new game because {nickName} was reconnected but the game does not support autoresume");
            await aggregator.PublishAsync(new EndGameEarlyEventModel());
            await _gameSetUp!.Network!.EndGameEarlyAsync(nickName);
            await aggregator.PublishAsync(new NewGameEventModel());
            return;
        }
        toast.ShowInfoToast($"{nickName} was reconnected");
        await aggregator.PublishAsync(new RestoreEventModel());
    }
}