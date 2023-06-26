namespace ClueBoardGame.Core.ViewModels;
[InstanceGame]
public partial class ClueBoardGameMainViewModel : BoardDiceGameVM, IHandleAsync<SelectionChosenEventModel>
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
        VMData.HandList.ObjectClickedAsync = HandList_ObjectClickedAsync;
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
    private bool _wasManuel;
    private async Task MoveSpaceAsync(int space)
    {
        if (_gameContainer.Test.DoubleCheck)
        {
            _gameContainer.TempClicked = space;
            _gameBoard.RepaintBoard();
            ManuallyEnable();
            return;
        }
        if (_gameBoard.CanMoveToSpace(space) == false)
        {
            ManuallyEnable();
            return;
        }
        if (_mainGame.SaveRoot.MovesLeft == 0)
        {
            ManuallyEnable();
            return;
        }
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("space", space);
        }
        _gameBoard.MoveToSpace(space);
        await _mainGame.ContinueMoveAsync();
    }
    public async Task LeftArrowAsync()
    {
        await ArrowNavigationAsync(EnumPositionInfo.Left);
    }
    public async Task RightArrowAsync()
    {
        await ArrowNavigationAsync(EnumPositionInfo.Right);
    }
    public async Task UpArrowAsync()
    {
        await ArrowNavigationAsync(EnumPositionInfo.Top);
    }
    public async Task DownArrowAsync()
    {
        await ArrowNavigationAsync(EnumPositionInfo.Bottom);
    }
    private async Task ArrowNavigationAsync(EnumPositionInfo direction)
    {
        //no need to send to other players.
        //because if you are able to make move, just send normal move anyways.
        //this is view alone.
        if (_gameContainer.Command.Processing)
        {
            return; //because everything should have been disabled.
        }
        if (_gameContainer.Command.IsExecuting)
        {
            return;
        }
        //if you are not on a space, can't do it.
        if (_gameContainer.SaveRoot.GameStatus != EnumClueStatusList.MoveSpaces)
        {
            return; //you can't do it because the status is not even move spaces.
        }
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            //only self can do it.
            return;
        }
        //if you can't make move, then can't do it.
        if (_gameContainer.CurrentCharacter!.CurrentRoom > 0)
        {
            return; //because you are currently in a room.
        }
        if (_gameContainer.CurrentCharacter.Space == 0)
        {
            if (_gameContainer.CurrentCharacter.FirstSpace == 41 && direction != EnumPositionInfo.Bottom) //space 1
            {
                return;
            }
            if ((_gameContainer.CurrentCharacter.FirstSpace == 15 || _gameContainer.CurrentCharacter.FirstSpace == 19) && direction != EnumPositionInfo.Right) //space 2 and 3
            {
                return;
            }
            if ((_gameContainer.CurrentCharacter.FirstSpace == 171 || _gameContainer.CurrentCharacter.FirstSpace == 172) && direction != EnumPositionInfo.Top) //space 4 and 5
            {
                return;
            }
            if (_gameContainer.CurrentCharacter.FirstSpace == 53 && direction != EnumPositionInfo.Left) //space 6
            {
                return;
            }
        }
        //int space = GetSpace(direction);
        _gameContainer.Command.StartExecuting(); //has to do manually
        int space = GetMove(direction);
        _wasManuel = true;
        if (space == 0)
        {
            ManuallyEnable();
            return;
        }
        await MoveSpaceAsync(space);
    }
    private int GetMove(EnumPositionInfo direction)
    {
        if (_gameContainer.CurrentCharacter!.Space > 0)
        {
            var currentField = _gameBoard.FieldList[_gameContainer.CurrentCharacter.Space];
            var output = currentField.Neighbors.Values.Where(x => x.Position == direction).SingleOrDefault();
            if (output is null)
            {
                return 0; //you for sure can't do it.
            }
            return output.SpaceNumber;
        }
        return _gameContainer.CurrentCharacter.FirstSpace;

    }
    private void ManuallyEnable()
    {
        if (_wasManuel == false)
        {
            return;
        }
        _wasManuel = false; //set to false again.
        _gameContainer.Command.StopExecuting();
    }
    private async Task MoveRoomAsync(int room)
    {
        if (_gameBoard.CanMoveToRoom(room) == false)
        {
            return;
        }
        if (_mainGame!.SaveRoot.GameStatus == EnumClueStatusList.EndTurn)
        {
            return; //because you are supposed to end turn.
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
        _mainGame.MarkCard(tempPlayer, payLoad, false);
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
        if (_mainGame.OtherTurn > 0)
        {
            return; //because you have to just give a clue alone.
        }
        VMData.CurrentRoomName = room.Name;
    }
    [Command(EnumCommandCategory.Game)]
    public void CurrentCharacterClick(DetectiveInfo character)
    {
        if (_mainGame.OtherTurn > 0)
        {
            return; //because you have to just give a clue alone.
        }
        VMData.CurrentCharacterName = character.Name;
    }

    [Command(EnumCommandCategory.Game)]
    public void CurrentWeaponClick(DetectiveInfo weapon) //had to try to change to detectiveinfo to support blazor.
    {
        if (_mainGame.OtherTurn > 0)
        {
            return; //because you have to just give a clue alone.
        }
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
    private bool AlreadyHasCardForAccusation()
    {
        var list = GetHand;
        foreach (var item in list.HandList)
        {
            if (item.Name == VMData.CurrentCharacterName)
            {
                return true;
            }
            if (item.Name == VMData.CurrentWeaponName)
            {
                return true;
            }
            if (item.Name == VMData.CurrentRoomName)
            {
                return true;
            }
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeAccusationAsync()
    {
        if (AlreadyHasCardForAccusation())
        {
            WarningEventModel warn = new();
            warn.Message = "Are you sure you want to make an accusation because you have one of the cards used for the clue";
            await Aggregator.PublishAsync(warn);
            return;
        }
        await FinishAccusationAsync();
    }
    private async Task FinishAccusationAsync()
    {

        _gameContainer!.SaveRoot!.CurrentPrediction!.CharacterName = VMData.CurrentCharacterName;
        _gameContainer.SaveRoot.CurrentPrediction.WeaponName = VMData.CurrentWeaponName;
        _gameContainer.SaveRoot.CurrentPrediction.RoomName = VMData.CurrentRoomName;
        await _mainGame.MakeAccusationAsync();
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
        if (_mainGame.OtherTurn > 0)
        {
            return; //because you have to just give a clue alone.
        }
        detective.IsChecked = !detective.IsChecked;
    }
    async Task IHandleAsync<SelectionChosenEventModel>.HandleAsync(SelectionChosenEventModel message)
    {
        switch (message.OptionChosen)
        {
            case EnumOptionChosen.Yes:
                await FinishAccusationAsync();
                break;
            case EnumOptionChosen.No:
                VMData.CurrentCharacterName = "";
                VMData.CurrentWeaponName = "";
                VMData.CurrentRoomName = "";
                _gameContainer!.SaveRoot!.CurrentPrediction!.CharacterName = "";
                _gameContainer.SaveRoot.CurrentPrediction.WeaponName = "";
                _gameContainer.SaveRoot.CurrentPrediction.RoomName = "";
                _gameContainer.Command.ManuelFinish = false;
                _gameContainer.Command.IsExecuting = false;
                _gameContainer.Command.UpdateAll();
                break;
            default:
                throw new CustomBasicException("Should have chosen yes or no");
        }
    }
}