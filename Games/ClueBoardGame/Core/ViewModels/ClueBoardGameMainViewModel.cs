namespace ClueBoardGame.Core.ViewModels;
[InstanceGame]
public partial class ClueBoardGameMainViewModel : BoardDiceGameVM
{
    private readonly ClueBoardGameMainGameClass _mainGame; //if we don't need, delete.
    public ClueBoardGameVMData VMData { get; set; }
    private readonly ClueBoardGameGameContainer _gameContainer;
    private readonly GameBoardProcesses _gameBoard;
    public ClueBoardGameMainViewModel(CommandContainer commandContainer,
        ClueBoardGameMainGameClass mainGame,
        ClueBoardGameVMData model,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        ClueBoardGameGameContainer gameContainer,
        GameBoardProcesses gameBoard,
        IEventAggregator aggregator
        )
        : base(commandContainer, mainGame, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = model;
        _gameContainer = gameContainer;
        _gameBoard = gameBoard;
        _gameContainer.SpaceClickedAsync = MoveSpaceAsync;
        _gameContainer.RoomClickedAsync = MoveRoomAsync;
        VMData.Pile.SendEnableProcesses(this, () => false);
        VMData.HandList.ObjectClickedAsync += HandList_ObjectClickedAsync;
        VMData.HandList.SendEnableProcesses(this, () => _mainGame.SaveRoot.GameStatus == EnumClueStatusList.FindClues);
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public void PopulateDetectiveNoteBook()
    {
        _mainGame.PopulateDetectiveNoteBook();
    }
    public Dictionary<int, DetectiveInfo> GetOwnDetectiveNotebook()
    {
        var player = _mainGame.PlayerList.GetSelf();
        return player.DetectiveList;
    }
    public HandObservable<CardInfo> GetHand => VMData.HandList;
    public SingleObservablePile<CardInfo> GetPile => VMData.Pile;
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    private async Task MoveSpaceAsync(int space)
    {
        if (_gameContainer.Test.DoubleCheck)
        {
            _gameContainer.TempClicked = space;
            _gameBoard.RepaintBoard();
            return;
        }
        if (_gameBoard.CanMoveToSpace(space) == false)
        {
            return;
        }
        if (_mainGame.SaveRoot.MovesLeft == 0)
        {
            return;
        }
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("space", space);
        }
        _gameBoard.MoveToSpace(space);
        await _mainGame.ContinueMoveAsync();
    }

    private async Task MoveRoomAsync(int room)
    {
        if (_gameBoard.CanMoveToRoom(room) == false)
        {
            return;
        }
        if (_mainGame!.SaveRoot!.GameStatus == EnumClueStatusList.MoveSpaces && _gameContainer.CurrentCharacter!.PreviousRoom > 0)
        {
            return;
        }
        if (_gameContainer.BasicData!.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("room", room);
        }
        _gameContainer.SaveRoot.MovesLeft = 0;
        _gameBoard.MoveToRoom(room);
        _gameContainer.SaveRoot.GameStatus = EnumClueStatusList.MakePrediction; //hopefully this is it.
    }
    private async Task HandList_ObjectClickedAsync(CardInfo payLoad, int index)
    {
        if (_gameContainer.CanGiveCard(payLoad) == false)
        {
            return;
        }
        payLoad.IsSelected = true;
        CommandContainer.UpdateAll(); //to notify blazor.
        if (_gameContainer.Test.NoAnimations == false)
        {
            await _gameContainer.Delay.DelaySeconds(.25);
        }
        var tempPlayer = _gameContainer!.PlayerList!.GetWhoPlayer();
        if (_gameContainer.BasicData!.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("cluegiven", payLoad.Deck);
            //has to send to everybody.  that way the host can record (since recording must be done).
            //await _gameContainer.Network!.SendToParticularPlayerAsync("cluegiven", payLoad.Deck, tempPlayer.NickName);
        }
        CommandContainer!.ManuelFinish = true;
        payLoad.IsSelected = false;
        _mainGame.MarkCard(tempPlayer, payLoad);
        _gameContainer.SaveRoot!.GameStatus = EnumClueStatusList.EndTurn;
        if (_gameContainer.BasicData.MultiPlayer == false)
        {
            throw new CustomBasicException("Computer should have never had this");
        }
        _gameContainer.Network!.IsEnabled = true; //to wait for them to end turn.
    }
    [Command(EnumCommandCategory.Game)]
    public void CurrentRoomClick(DetectiveInfo room)
    {
        VMData.CurrentRoomName = room.Name;
    }
    [Command(EnumCommandCategory.Game)]
    public void CurrentCharacterClick(DetectiveInfo character)
    {
        VMData.CurrentCharacterName = character.Name;
    }

    [Command(EnumCommandCategory.Game)]
    public void CurrentWeaponClick(DetectiveInfo weapon) //had to try to change to detectiveinfo to support blazor.
    {
        VMData.CurrentWeaponName = weapon.Name;
    }
    public bool CanStartOver()
    {
        if (_gameContainer.SaveRoot.GameStatus == EnumClueStatusList.StartTurn 
            || _gameContainer.SaveRoot.GameStatus == EnumClueStatusList.FindClues
            || _gameContainer.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction
            || _gameContainer.SaveRoot.GameStatus == EnumClueStatusList.EndGame)
        {
            return false;
        }
        if (_gameContainer.SaveRoot.GameStatus == EnumClueStatusList.EndTurn)
        {
            return _gameContainer.CurrentCharacter!.CurrentRoom == 0; //if you made it into the room, then you can't.
        }
        //has to move one space.  or its not even worth doing it.
        if (VMData.LeftToMove == VMData.Cup!.ValueOfOnlyDice)
        {
            return false;
        }
        return true;
        //return _gameContainer.SaveRoot.GameStatus == EnumClueStatusList.MoveSpaces;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task StartOverAsync()
    {
        //see if i can figure out when it needs to enable it.

        //VMData.LeftToMove = VMData.Cup!.ValueOfOnlyDice;
        //start out with extending move (will eventually rethink).
        if (_gameContainer.BasicData.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("startover");
        }
        await _mainGame.StartOverAsync();
    }
    public bool CanMakePrediction
    {
        get
        {
            if (VMData.CurrentWeaponName == "" || VMData.CurrentCharacterName == "")
            {
                return false;
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction)
            {
                return true;
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumClueStatusList.StartTurn)
            {
                if (_gameContainer.CurrentCharacter!.PreviousRoom != _gameContainer.CurrentCharacter.CurrentRoom)
                {
                    return true;
                }
            }
            return false;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakePredictionAsync()
    {
        _gameContainer!.SaveRoot!.CurrentPrediction!.CharacterName = VMData.CurrentCharacterName;
        _gameContainer.SaveRoot.CurrentPrediction.WeaponName = VMData.CurrentWeaponName;
        await _mainGame.MakePredictionAsync();
    }
    public bool CanMakeAccusation
    {
        get
        {
            if (_gameContainer.SaveRoot.GameStatus == EnumClueStatusList.FindClues)
            {
                return false;
            }
            if (VMData.CurrentWeaponName == "" || VMData.CurrentCharacterName == "" || VMData.CurrentRoomName == "")
            {
                return false;
            }
            return true;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeAccusationAsync()
    {
        _gameContainer!.SaveRoot!.CurrentPrediction!.CharacterName = VMData.CurrentCharacterName;
        _gameContainer.SaveRoot.CurrentPrediction.WeaponName = VMData.CurrentWeaponName;
        _gameContainer.SaveRoot.CurrentPrediction.RoomName = VMData.CurrentRoomName;
        await _mainGame.MakeAccusationAsync();
    }
    protected override Task TryCloseAsync()
    {
        VMData.HandList.ObjectClickedAsync -= HandList_ObjectClickedAsync;
        return base.TryCloseAsync();
    }
    public override bool CanRollDice()
    {
        return _gameContainer.SaveRoot.GameStatus == EnumClueStatusList.StartTurn;
    }
    public override bool CanEndTurn()
    {
        return _gameContainer.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction || _gameContainer.SaveRoot.GameStatus == EnumClueStatusList.EndTurn;
    }
    public override async Task RollDiceAsync()
    {
        await base.RollDiceAsync();
    }
    [Command(EnumCommandCategory.Limited)]
    public void FillInClue(DetectiveInfo detective)
    {
        if (_gameContainer is null)
        {
            throw new CustomBasicException("Must have container");
        }
        detective.IsChecked = !detective.IsChecked;
    }
}