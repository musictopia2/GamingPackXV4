namespace BasicGameFrameworkLibrary.Core.ViewModels;
[UseLabelGrid]
public partial class MultiplayerOpeningViewModel<P>
    (CommandContainer commandContainer,
        IMultiplayerSaveState state,
        BasicData data,
        IGameNetwork nets,
        TestOptions test,
        IGameInfo game,
        IEventAggregator aggregator,
        IMessageBox message)
    : IMultiplayerOpeningVM<P>,
    IMultiplayerOpeningDisplayVM, IReadyNM,
    IOpeningMessenger,
    IHandleAsync<OpenMultiplayerOpeningEventModel<P>>,
    IHandleAsync<CloseMultiplayerOpeningEventModel<P>>,
    IStateChanged

    where P : class, IPlayerItem, new()
{
    private EnumRestoreCategory _singleRestore;
    private EnumRestoreCategory _multiRestore;
    private PlayerCollection<P> _playerList = new();
    private PlayerCollection<P>? _saveList;
    private bool _disconnectedClients;
    private bool _rejoin;
    private partial void Subscribe();
    private partial void Unsubscribe();
    partial void CreateCommands(CommandContainer container);
    async Task IHandleAsync<OpenMultiplayerOpeningEventModel<P>>.HandleAsync(OpenMultiplayerOpeningEventModel<P> message)
    {
        _singleRestore = await state.SinglePlayerRestoreCategoryAsync();
        _multiRestore = await state.MultiplayerRestoreCategoryAsync();
        if (_multiRestore != EnumRestoreCategory.NoRestore)
        {
            IRetrieveSavedPlayers<P> rr = Resolver!.Resolve<IRetrieveSavedPlayers<P>>();
            _savedData = await state.TempMultiSavedAsync();
            if (_savedData != "")
            {
                _saveList = await rr.GetPlayerListAsync(_savedData);
                _saveList.RemoveNonHumanPlayers();
            }
        }
        OpeningStatus = EnumOpeningStatus.None;
        ShowOtherChangesBecauseOfNetworkChange();
        commandContainer.OpenBusy = false;
        StateChanged?.Invoke();
    }

    async Task IHandleAsync<CloseMultiplayerOpeningEventModel<P>>.HandleAsync(CloseMultiplayerOpeningEventModel<P> message)
    {
        Unsubscribe();
    }

    void IMultiplayerOpeningVM<P>.Initialize()
    {
        Subscribe();
        commandContainer.OpenBusy = true;
        CreateCommands(commandContainer);
    }
    public bool HasServer => nets.HasServer;
    private string _savedData = "";

    public event Action? StateChanged;

    public EnumOpeningStatus OpeningStatus { get; set; } = EnumOpeningStatus.None;
    private void Reset()
    {
        commandContainer.OpenBusy = false;
        OpeningStatus = EnumOpeningStatus.None;
        ShowOtherChangesBecauseOfNetworkChange();
        StateChanged?.Invoke();
    }
    #region "Command Options"
    public bool CanResumeSinglePlayer
    {
        get
        {
            if (OpeningStatus != EnumOpeningStatus.None)
            {
                return false;
            }
            return _singleRestore != EnumRestoreCategory.NoRestore;
        }
    }
    [Command(EnumCommandCategory.Open)]
    public async Task ResumeSinglePlayerAsync()
    {
        await StartSavedGameAsync();
    }
    public bool CanDisconnectEverybody()
    {
        return ClientsConnected > 0;
    }
    [Command(EnumCommandCategory.Open)]
    public async Task DisconnectEverybodyAsync()
    {
        _playerList.DisconnectEverybody();
        OpeningStatus = EnumOpeningStatus.HostingWaitingForAtLeastOnePlayer;
        await nets.DisconnectEverybodyAsync();
        commandContainer.OpenBusy = false;
        StateChanged?.Invoke();
    }
    public bool CanRejoinMultiplayerGame()
    {
        if (_saveList is null)
        {
            return false;
        }
        if (OpeningStatus != EnumOpeningStatus.None)
        {
            return false;
        }
        return _saveList.Count > 0;
    }
    [Command(EnumCommandCategory.Open)]
    public async Task RejoinMultiplayerGameAsync()
    {
        _rejoin = true;
        await HostAsync();
    }
    [Command(EnumCommandCategory.Open)]
    public async Task TransferToDesktopAsync()
    {
        TransferAutoResumeModel payLoad = new()
        {
            GameDisplayName = game.GameName,
            Content = _savedData
        };
        await GlobalDelegates.TransferToDesktop!.Invoke(payLoad);
        await state.DeleteMultiplayerGameAsync();
    }
    public bool CanResumeMultiplayerGame
    {
        get
        {
            if (OpeningStatus != EnumOpeningStatus.HostingReadyToStart)
            {
                return false;
            }
            if (_multiRestore == EnumRestoreCategory.NoRestore)
            {
                return false;
            }
            BasicList<P> temporaryList = _playerList.GetTemporaryList();
            if (_saveList == null)
            {
                throw new CustomBasicException("Save list was never created to figure out whether can resume multiplayer game.  Rethink");
            }
            return temporaryList.DoesReconcile(_saveList, Items => Items.NickName);
        }
    }
    [Command(EnumCommandCategory.Open)]
    public async Task ResumeMultiplayerGameAsync()
    {
        data.MultiPlayer = true;
        await StartSavedGameAsync();
    }
    public bool CanStartComputerSinglePlayerGame(int howMany)
    {
        if (howMany == 0)
        {
            return false;
        }
        if (OpeningStatus != EnumOpeningStatus.None)
        {
            return false;
        }
        if (_singleRestore == EnumRestoreCategory.MustRestore)
        {
            return false;
        }
        return OpenPlayersHelper.CanComputer(game);
    }

    [Command(EnumCommandCategory.Open)]
    public async Task StartComputerSinglePlayerGameAsync(int howManyComputerPlayers)
    {
        StartSingle();
        bool rets;
        if (test.PlayCategory == EnumTestPlayCategory.Reverse)
        {
            rets = true;
        }
        else
        {
            rets = false;
        }
        _playerList.LoadPlayers(1, howManyComputerPlayers, rets); //i think
        await StartNewGameAsync();
    }
    public bool CanStartPassAndPlayGame(int howMany)
    {
        if (howMany == 0)
        {
            return false;
        }
        if (OpeningStatus != EnumOpeningStatus.None)
        {
            return false;
        }
        if (_singleRestore == EnumRestoreCategory.MustRestore)
        {
            return false;
        }
        return OpenPlayersHelper.CanHuman(game);
    }
    [Command(EnumCommandCategory.Open)]
    public async Task StartPassAndPlayGameAsync(int howManyHumanPlayers)
    {
        StartSingle();
        _playerList.LoadPlayers(howManyHumanPlayers);
        await StartNewGameAsync();
    }
    public bool CanStart(int howManyExtra)
    {
        //return false;
        if (_multiRestore == EnumRestoreCategory.MustRestore)
        {
            return false;
        }
        int tempCount = _playerList.GetTemporaryCount;
        if (tempCount == 0)
        {
            return false;
        }
        if (howManyExtra > 0 && game.CanHaveExtraComputerPlayers == false)
        {
            return false;
        }
        if (howManyExtra > 0)
        {
            if (howManyExtra + tempCount > game.MaxPlayers)
            {
                return false;
            }
            if (howManyExtra + tempCount < game.MinPlayers)
            {
                return false;
            }
            if (howManyExtra + tempCount == game.NoPlayers)
            {
                return false;
            }
        }
        if (howManyExtra == 0)
        {
            if (tempCount < game.MinPlayers)
            {
                return false; //because not enough players.
            }
        }
        return OpeningStatus == EnumOpeningStatus.HostingReadyToStart;
    }
    [Command(EnumCommandCategory.Open)]
    public async Task StartAsync(int howManyExtra)
    {
        if (howManyExtra > 0)
        {
            _playerList.LoadPlayers(0, howManyExtra);
        }
        data.MultiPlayer = true;
        await StartNewGameAsync();
    }
    public bool CanHost => OpeningStatus == EnumOpeningStatus.None;

    [Command(EnumCommandCategory.Open)]
    public async Task HostAsync()
    {
        bool rets = await nets.InitNetworkMessagesAsync(data.NickName, false);
        if (rets == false)
        {
            await message.ShowMessageAsync("Failed To Connect To Server");
            Reset();
            return;
        }
    }
    public bool CanConnect => OpeningStatus == EnumOpeningStatus.None;
    [Command(EnumCommandCategory.Open)]
    public async Task ConnectAsync()
    {
        commandContainer.OpenBusy = true;
        await Task.Delay(20);
        bool rets = await nets.InitNetworkMessagesAsync(data.NickName, true);
        if (rets == false)
        {
            await message.ShowMessageAsync("Failed To Connect To Server");
            Reset();
            return;
        }
        data.DoFullScreen?.Invoke();
        await nets.ConnectToHostAsync();
        StateChanged?.Invoke();
    }
    public bool CanSolitaire => game.SinglePlayerChoice == EnumPlayerChoices.Solitaire && OpeningStatus == EnumOpeningStatus.None;
    [Command(EnumCommandCategory.Open)]
    public async Task SolitaireAsync()
    {
        StartSingle();
        await StartNewGameAsync();
    }
    public bool CanCancelConnection => OpeningStatus == EnumOpeningStatus.HostingWaitingForAtLeastOnePlayer || OpeningStatus == EnumOpeningStatus.WaitingForHost;
    [Command(EnumCommandCategory.Open)]
    public Task CancelConnectionAsync()
    {
        OpeningStatus = EnumOpeningStatus.None;
        PrivateClose();
        commandContainer.OpenBusy = false;
        StateChanged?.Invoke();
        return Task.CompletedTask;
    }
    private async void PrivateClose()
    {
        await nets.CloseConnectionAsync();
    }
    #endregion
    private void ShowOtherChangesBecauseOfNetworkChange()
    {
        if (OpeningStatus == EnumOpeningStatus.HostingReadyToStart)
        {
            ExtraOptionsVisible = game.CanHaveExtraComputerPlayers;
        }
        else
        {
            ExtraOptionsVisible = false;
        }
    }



    #region "Properties"
    public bool ExtraOptionsVisible { get; set; }
    [LabelColumn]
    public int ClientsConnected
    {
        get
        {
            if (_playerList.GetTemporaryCount == 0)
            {
                return 0;
            }
            return _playerList.GetTemporaryCount - 1;
        }
    }
    public bool HostCanStart => OpeningStatus == EnumOpeningStatus.HostingReadyToStart;
    public bool CanShowTransferToDesktop()
    {
        if (GlobalDelegates.TransferToDesktop is null)
        {
            return false;
        }
        if (HostCanStart == true)
        {
            return false;
        }
        return CanRejoinMultiplayerGame();
    }
    public bool CanShowSingleOptions => OpeningStatus == EnumOpeningStatus.None;
    [LabelColumn]
    public partial int PreviousNonComputerNetworkedPlayers { get; set; }
    #endregion
    private void LoadNewGamePlayers()
    {
        foreach (var item in NewGameContainer.NewGameHost!.Players)
        {
            P player = new()
            {
                Id = item.Id,
                NickName = item.NickName,
                IsHost = item.IsHost,
                PlayerCategory = item.PlayerCategory,
                IsReady = true,
                InGame = true
            };
            if (player.CanStartInGame == false)
            {
                player.InGame = false;
            }
            _playerList.AddPlayer(player);
        }
    }
    public async Task StartAnotherSinglePlayerGameAsync()
    {
        //this means starting another game.
        if (NewGameContainer.NewGameHost is null)
        {
            throw new CustomBasicException("Cannot start another game because has no information to load another game based on information from previous game");
        }
        if (NewGameContainer.NewGameHost.Multiplayer)
        {
            throw new CustomBasicException("Only single player games should call this method");
        }
        StartSingle();
        LoadNewGamePlayers();
        await StartNewGameAsync();
        return; //this is the easist
    }
    private void StartSingle()
    {
        data.MultiPlayer = false;
        _playerList = new PlayerCollection<P>();
    }
    private async Task StartNewGameAsync()
    {
        await state.DeleteGameAsync();
        await aggregator.PublishAsync(new StartMultiplayerGameEventModel<P>(_playerList));
    }
    private async Task StartSavedGameAsync()
    {
        await aggregator.PublishAsync(new StartAutoresumeMultiplayerGameEventModel());
    }
    async Task IOpeningMessenger.ConnectedToHostAsync(IGameNetwork network, string hostName)
    {
        await nets.SendReadyMessageAsync(network.NickName, hostName);
        await FinishWaitingForHostAsync(network);
    }
    private async Task FinishWaitingForHostAsync(IGameNetwork thisCheck)
    {
        OpeningStatus = EnumOpeningStatus.ConnectedToHost;
        data.Client = true;
        data.MultiPlayer = true;
        data.NickName = thisCheck.NickName;
        thisCheck.IsEnabled = true;
        ShowOtherChangesBecauseOfNetworkChange();
        await aggregator.PublishAsync(new WaitForHostEventModel());
        StateChanged?.Invoke();
        //commandContainer.UpdateAll();
    }
    async Task IOpeningMessenger.HostConnectedAsync(IGameNetwork network)
    {
        data.Client = false;
        if (_rejoin)
        {
            await ResumeMultiplayerGameAsync();
            return;
        }

        network.IsEnabled = true;
        _playerList = new();
        AddHostPlayer();
        if (_saveList != null)
        {
            PreviousNonComputerNetworkedPlayers = _saveList.Count - 1;
        }
        OpeningStatus = EnumOpeningStatus.HostingWaitingForAtLeastOnePlayer;
        commandContainer.OpenBusy = false;
        StateChanged?.Invoke();
    }
    private void AddHostPlayer()
    {
        P thisPlayer = new();
        thisPlayer.NickName = data.NickName;
        thisPlayer.IsHost = true;
        thisPlayer.Id = 1;
        thisPlayer.InGame = true;
        thisPlayer.PlayerCategory = EnumPlayerCategory.OtherHuman;
        _playerList.AddPlayer(thisPlayer);
    }
    async Task IReadyNM.ProcessReadyAsync(string nickName)
    {
        commandContainer.OpenBusy = true;
        P thisPlayer = new();
        thisPlayer.NickName = nickName;
        thisPlayer.IsHost = false;
        thisPlayer.Id = _playerList.GetTemporaryCount + 1;
        thisPlayer.InGame = true;
        thisPlayer.PlayerCategory = EnumPlayerCategory.OtherHuman;
        _playerList.AddPlayer(thisPlayer);
        if (NewGameContainer.NewGameHost is not null)
        {
            var computers = NewGameContainer.NewGameHost.Players.Count(x => x.PlayerCategory == EnumPlayerCategory.Computer);
            var count = _playerList.GetTemporaryCount + computers;
            if (count == NewGameContainer.NewGameHost.Players.Count)
            {
                if (_disconnectedClients == false)
                {
                    await nets.NewGameAsync();
                    _disconnectedClients = true;
                    _playerList.ClearTempPlayers();
                    AddHostPlayer();
                    OpeningStatus = EnumOpeningStatus.WaitingForOtherPlayersForNewGame; //try this way.
                    ShowOtherChangesBecauseOfNetworkChange();
                    nets.IsEnabled = true;
                    commandContainer.OpenBusy = false;
                    return; //well see.
                }
                data.MultiPlayer = true; //try this too.
                _playerList.ClearTempPlayers(); //i think.  so this can reload in the proper order.
                LoadNewGamePlayers();
                await StartNewGameAsync();
                return;
            }
            OpeningStatus = EnumOpeningStatus.WaitingForOtherPlayersForNewGame;
        }
        else
        {
            OpeningStatus = EnumOpeningStatus.HostingReadyToStart;
        }
        ShowOtherChangesBecauseOfNetworkChange();
        nets.IsEnabled = true; //try this.
        commandContainer.OpenBusy = false;
        StateChanged?.Invoke(); //try this too.
    }
    Task IOpeningMessenger.WaitingForHostAsync(IGameNetwork network)
    {
        return ClientChangeStatus(EnumOpeningStatus.WaitingForHost, network);
    }
    private Task ClientChangeStatus(EnumOpeningStatus status, IGameNetwork network)
    {
        ShowOtherChangesBecauseOfNetworkChange();
        data.Client = true;
        data.MultiPlayer = true;
        data.NickName = network.NickName;
        network.IsEnabled = true;
        commandContainer.OpenBusy = false;
        OpeningStatus = status;
        StateChanged?.Invoke();
        return Task.CompletedTask;
    }
    Task IOpeningMessenger.WaitForGameAsync(IGameNetwork network)
    {
        return FinishWaitingForHostAsync(network);
    }

}