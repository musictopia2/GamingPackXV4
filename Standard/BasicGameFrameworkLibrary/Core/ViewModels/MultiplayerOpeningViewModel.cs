namespace BasicGameFrameworkLibrary.Core.ViewModels;
[UseLabelGrid]
public partial class MultiplayerOpeningViewModel<P> : ScreenViewModel, IBlankGameVM, IOpeningMessenger, IReadyNM, IMultiplayerOpeningViewModel where P : class, IPlayerItem, new()
{
    private readonly IMultiplayerSaveState _state;
    private readonly BasicData _data;
    private readonly IGameNetwork _nets;
    private readonly TestOptions _test;
    private readonly IGameInfo _game;
    private readonly IMessageBox _message;
    private EnumRestoreCategory _singleRestore;
    private EnumRestoreCategory _multiRestore;
    private PlayerCollection<P> _playerList = new();
    private PlayerCollection<P>? _saveList;
    private bool _disconnectedClients;
    private bool _rejoin;
    public MultiplayerOpeningViewModel(CommandContainer commandContainer,
        IMultiplayerSaveState thisState,
        BasicData data,
        IGameNetwork nets,
        TestOptions test,
        IGameInfo game,
        IEventAggregator aggregator,
        IMessageBox message
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        CommandContainer.OpenBusy = true;
        _state = thisState;
        _data = data;
        _nets = nets;
        _test = test;
        _game = game;
        _message = message;
        CreateCommands(commandContainer);
    }
    public bool HasServer => _nets.HasServer;
    partial void CreateCommands(CommandContainer container);
    private string _savedData = "";
    protected override async Task ActivateAsync()
    {
        _singleRestore = await _state.SinglePlayerRestoreCategoryAsync();
        _multiRestore = await _state.MultiplayerRestoreCategoryAsync();
        if (_multiRestore != EnumRestoreCategory.NoRestore)
        {
            IRetrieveSavedPlayers<P> rr = Resolver!.Resolve<IRetrieveSavedPlayers<P>>();
            _savedData = await _state.TempMultiSavedAsync();
            if (_savedData != "")
            {
                _saveList = await rr.GetPlayerListAsync(_savedData);
                _saveList.RemoveNonHumanPlayers();
            }
        }
        OpeningStatus = EnumOpeningStatus.None;
        ShowOtherChangesBecauseOfNetworkChange();
        await base.ActivateAsync();
        CommandContainer.OpenBusy = false;
    }
    public CommandContainer CommandContainer { get; set; }
    public EnumOpeningStatus OpeningStatus { get; set; } = EnumOpeningStatus.None;
    private void Reset()
    {
        CommandContainer.OpenBusy = false;
        OpeningStatus = EnumOpeningStatus.None;
        ShowOtherChangesBecauseOfNetworkChange();
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
        await _nets.DisconnectEverybodyAsync();
        CommandContainer.OpenBusy = false;
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
            GameDisplayName = _game.GameName,
            Content = _savedData
        };
        await GlobalDelegates.TransferToDesktop!.Invoke(payLoad);
        await _state.DeleteMultiplayerGameAsync();
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
        _data.MultiPlayer = true;
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
        return OpenPlayersHelper.CanComputer(_game);
    }

    [Command(EnumCommandCategory.Open)]
    public async Task StartComputerSinglePlayerGameAsync(int howManyComputerPlayers)
    {
        StartSingle();
        bool rets;
        if (_test.PlayCategory == EnumTestPlayCategory.Reverse)
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
        return OpenPlayersHelper.CanHuman(_game);
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
        if (_multiRestore == EnumRestoreCategory.MustRestore)
        {
            return false;
        }
        int tempCount = _playerList.GetTemporaryCount;
        if (tempCount == 0)
        {
            return false;
        }
        if (howManyExtra > 0 && _game.CanHaveExtraComputerPlayers == false)
        {
            return false;
        }
        if (howManyExtra > 0)
        {
            if (howManyExtra + tempCount > _game.MaxPlayers)
            {
                return false;
            }
            if (howManyExtra + tempCount < _game.MinPlayers)
            {
                return false;
            }
            if (howManyExtra + tempCount == _game.NoPlayers)
            {
                return false;
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
        _data.MultiPlayer = true;
        await StartNewGameAsync();
    }
    public bool CanHost => OpeningStatus == EnumOpeningStatus.None;

    [Command(EnumCommandCategory.Open)]
    public async Task HostAsync()
    {
        bool rets = await _nets.InitNetworkMessagesAsync(_data.NickName, false);
        if (rets == false)
        {
            await _message.ShowMessageAsync("Failed To Connect To Server");
            Reset();
            return;
        }
    }
    public bool CanConnect => OpeningStatus == EnumOpeningStatus.None;
    [Command(EnumCommandCategory.Open)]
    public async Task ConnectAsync()
    {
        CommandContainer.OpenBusy = true;
        CommandContainer.UpdateAll();
        await Task.Delay(20);
        bool rets = await _nets.InitNetworkMessagesAsync(_data.NickName, true);
        if (rets == false)
        {
            await _message.ShowMessageAsync("Failed To Connect To Server");
            Reset();
            return;
        }
        _data.DoFullScreen?.Invoke();
        await _nets.ConnectToHostAsync();
    }
    public bool CanSolitaire => _game.SinglePlayerChoice == EnumPlayerChoices.Solitaire && OpeningStatus == EnumOpeningStatus.None;
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
        CommandContainer.OpenBusy = false;
        return Task.CompletedTask;
    }
    private async void PrivateClose()
    {
        await _nets.CloseConnectionAsync();
    }
    #endregion
    private void ShowOtherChangesBecauseOfNetworkChange()
    {
        if (OpeningStatus == EnumOpeningStatus.HostingReadyToStart)
        {
            ExtraOptionsVisible = _game.CanHaveExtraComputerPlayers;
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
    public int PreviousNonComputerNetworkedPlayers { get; set; }
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
        _data.MultiPlayer = false;
        _playerList = new PlayerCollection<P>();
    }
    private async Task StartNewGameAsync()
    {
        await _state.DeleteGameAsync();
        await Aggregator.PublishAsync(new StartMultiplayerGameEventModel<P>(_playerList));
    }
    private async Task StartSavedGameAsync()
    {
        await Aggregator.PublishAsync(new StartAutoresumeMultiplayerGameEventModel());
    }
    async Task IOpeningMessenger.ConnectedToHostAsync(IGameNetwork network, string hostName)
    {
        await _nets!.SendReadyMessageAsync(network.NickName, hostName);
        await FinishWaitingForHostAsync(network);
    }
    private async Task FinishWaitingForHostAsync(IGameNetwork thisCheck)
    {
        OpeningStatus = EnumOpeningStatus.ConnectedToHost;
        _data.Client = true;
        _data.MultiPlayer = true;
        _data.NickName = thisCheck.NickName;
        thisCheck.IsEnabled = true;
        ShowOtherChangesBecauseOfNetworkChange();
        await Aggregator.PublishAsync(new WaitForHostEventModel());
        CommandContainer.UpdateAll();
    }
    async Task IOpeningMessenger.HostConnectedAsync(IGameNetwork network)
    {
        _data.Client = false;
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
        CommandContainer.OpenBusy = false;
    }
    private void AddHostPlayer()
    {
        AddHostPlayer();
        P thisPlayer = new();
        thisPlayer.NickName = _data.NickName;
        thisPlayer.IsHost = true;
        thisPlayer.Id = 1;
        thisPlayer.InGame = true;
        thisPlayer.PlayerCategory = EnumPlayerCategory.OtherHuman;
        _playerList.AddPlayer(thisPlayer);
    }
    async Task IReadyNM.ProcessReadyAsync(string nickName)
    {
        CommandContainer.OpenBusy = true;
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
                    await _nets.NewGameAsync();
                    _disconnectedClients = true;
                    _playerList.ClearTempPlayers();
                    AddHostPlayer();
                    OpeningStatus = EnumOpeningStatus.WaitingForOtherPlayersForNewGame; //try this way.
                    ShowOtherChangesBecauseOfNetworkChange();
                    _nets.IsEnabled = true;
                    CommandContainer.OpenBusy = false;
                    return; //well see.
                }
                _data.MultiPlayer = true; //try this too.
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
        _nets.IsEnabled = true; //try this.
        CommandContainer.OpenBusy = false;
    }
    Task IOpeningMessenger.WaitingForHostAsync(IGameNetwork network)
    {
        return ClientChangeStatus(EnumOpeningStatus.WaitingForHost, network);
    }
    private Task ClientChangeStatus(EnumOpeningStatus status, IGameNetwork network)
    {
        ShowOtherChangesBecauseOfNetworkChange();
        _data.Client = true;
        _data.MultiPlayer = true;
        _data.NickName = network.NickName;
        network.IsEnabled = true;
        CommandContainer.OpenBusy = false;
        OpeningStatus = status;
        CommandContainer.UpdateAll();
        return Task.CompletedTask;
    }
    Task IOpeningMessenger.WaitForGameAsync(IGameNetwork network)
    {
        return FinishWaitingForHostAsync(network);
    }
}