namespace ClueBoardGame.Core.ViewModels;
[InstanceGame]
[EventAggregatorFromMemberClass(nameof(Aggregator))]
public partial class ClueBoardGameMainViewModel(CommandContainer commandContainer,
        ClueBoardGameMainGameClass mainGame,
        ClueBoardGameVMData model,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        ClueBoardGameGameContainer gameContainer,
        GameBoardProcesses gameBoard,
        IEventAggregator aggregator) : BoardDiceGameVM(commandContainer, mainGame, basicData, resolver, roller, aggregator),
    IHandleAsync<SelectionChosenEventModel>
{
    public ClueBoardGameVMData VMData { get; set; } = model;
    private partial void Subscribe();
    private partial void Unsubscribe();
    protected override void AdditionalInitialize()
    {
        Subscribe();
        gameContainer.SpaceClickedAsync = MoveSpaceAsync;
        gameContainer.RoomClickedAsync = MoveRoomAsync;
        VMData.Pile.SendEnableProcesses(this, () => false);
        VMData.HandList.ObjectClickedAsync = HandList_ObjectClickedAsync;
        VMData.HandList.SendEnableProcesses(this, () => mainGame.SaveRoot.GameStatus == EnumClueStatusList.FindClues);
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        Unsubscribe();
        VMData.HandList.ObjectClickedAsync = null;
        return base.CloseAsync();
    }
    partial void CreateCommands(CommandContainer command);
    public void PopulateDetectiveNoteBook()
    {
        mainGame.PopulateDetectiveNoteBook();
    }
    public Dictionary<int, DetectiveInfo> GetOwnDetectiveNotebook()
    {
        var player = mainGame.PlayerList.GetSelf();
        return player.DetectiveList;
    }
    public HandObservable<CardInfo> GetHand => VMData.HandList;
    public SingleObservablePile<CardInfo> GetPile => VMData.Pile;
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    private bool _wasManuel;
    private async Task MoveSpaceAsync(int space)
    {
        if (gameContainer.Test.DoubleCheck)
        {
            gameContainer.TempClicked = space;
            gameBoard.RepaintBoard();
            ManuallyEnable();
            return;
        }
        if (gameBoard.CanMoveToSpace(space) == false)
        {
            ManuallyEnable();
            return;
        }
        if (mainGame.SaveRoot.MovesLeft == 0)
        {
            ManuallyEnable();
            return;
        }
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("space", space);
        }
        gameBoard.MoveToSpace(space);
        await mainGame.ContinueMoveAsync();
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
        if (gameContainer.Command.Processing)
        {
            return; //because everything should have been disabled.
        }
        if (gameContainer.Command.IsExecuting)
        {
            return;
        }
        //if you are not on a space, can't do it.
        if (gameContainer.SaveRoot.GameStatus != EnumClueStatusList.MoveSpaces)
        {
            return; //you can't do it because the status is not even move spaces.
        }
        if (gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            //only self can do it.
            return;
        }
        //if you can't make move, then can't do it.
        if (gameContainer.CurrentCharacter!.CurrentRoom > 0)
        {
            return; //because you are currently in a room.
        }
        if (gameContainer.CurrentCharacter.Space == 0)
        {
            if (gameContainer.CurrentCharacter.FirstSpace == 41 && direction != EnumPositionInfo.Bottom) //space 1
            {
                return;
            }
            if ((gameContainer.CurrentCharacter.FirstSpace == 15 || gameContainer.CurrentCharacter.FirstSpace == 19) && direction != EnumPositionInfo.Right) //space 2 and 3
            {
                return;
            }
            if ((gameContainer.CurrentCharacter.FirstSpace == 171 || gameContainer.CurrentCharacter.FirstSpace == 172) && direction != EnumPositionInfo.Top) //space 4 and 5
            {
                return;
            }
            if (gameContainer.CurrentCharacter.FirstSpace == 53 && direction != EnumPositionInfo.Left) //space 6
            {
                return;
            }
        }
        //int space = GetSpace(direction);
        gameContainer.Command.StartExecuting(); //has to do manually
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
        if (gameContainer.CurrentCharacter!.Space > 0)
        {
            var currentField = gameBoard.FieldList[gameContainer.CurrentCharacter.Space];
            var output = currentField.Neighbors.Values.Where(x => x.Position == direction).SingleOrDefault();
            if (output is null)
            {
                return 0; //you for sure can't do it.
            }
            return output.SpaceNumber;
        }
        return gameContainer.CurrentCharacter.FirstSpace;

    }
    private void ManuallyEnable()
    {
        if (_wasManuel == false)
        {
            return;
        }
        _wasManuel = false; //set to false again.
        gameContainer.Command.StopExecuting();
    }
    private async Task MoveRoomAsync(int room)
    {
        if (gameBoard.CanMoveToRoom(room) == false)
        {
            return;
        }
        if (mainGame!.SaveRoot.GameStatus == EnumClueStatusList.EndTurn)
        {
            return; //because you are supposed to end turn.
        }
        if (mainGame!.SaveRoot!.GameStatus == EnumClueStatusList.MoveSpaces && gameContainer.CurrentCharacter!.PreviousRoom > 0)
        {
            return;
        }
        if (gameContainer.BasicData!.MultiPlayer)
        {
            await gameContainer.Network!.SendAllAsync("room", room);
        }
        gameContainer.SaveRoot.MovesLeft = 0;
        gameBoard.MoveToRoom(room);
        gameContainer.SaveRoot.GameStatus = EnumClueStatusList.MakePrediction; //hopefully this is it.
    }
    private async Task HandList_ObjectClickedAsync(CardInfo payLoad, int index)
    {
        if (gameContainer.CanGiveCard(payLoad) == false)
        {
            return;
        }
        payLoad.IsSelected = true;
        if (gameContainer.Test.NoAnimations == false)
        {
            await gameContainer.Delay.DelaySeconds(.25);
        }
        var player = gameContainer.PlayerList!.GetSelf();
       
        var tempPlayer = gameContainer!.PlayerList!.GetWhoPlayer();
        if (gameContainer.BasicData!.MultiPlayer)
        {
            HintInfo hint = new()
            {
                Deck = payLoad.Deck,
                NickName = player.NickName
            };
            await gameContainer.Network!.SendAllAsync("cluegiven", hint);
            //only whoever turn it is needs to show who gave it.
            //has to send to everybody.  that way the host can record (since recording must be done).
            //await gameContainer.Network!.SendToParticularPlayerAsync("cluegiven", payLoad.Deck, tempPlayer.NickName);
        }
        CommandContainer!.ManuelFinish = true;
        payLoad.IsSelected = false;
        mainGame.MarkCard(tempPlayer, payLoad, false);
        gameContainer.SaveRoot!.GameStatus = EnumClueStatusList.EndTurn;
        if (gameContainer.BasicData.MultiPlayer == false)
        {
            throw new CustomBasicException("Computer should have never had this");
        }
        gameContainer.Network!.IsEnabled = true; //to wait for them to end turn.
    }
    [Command(EnumCommandCategory.Game)]
    public void CurrentRoomClick(DetectiveInfo room)
    {
        if (mainGame.OtherTurn > 0)
        {
            return; //because you have to just give a clue alone.
        }
        VMData.CurrentRoomName = room.Name;
    }
    [Command(EnumCommandCategory.Game)]
    public void CurrentCharacterClick(DetectiveInfo character)
    {
        if (mainGame.OtherTurn > 0)
        {
            return; //because you have to just give a clue alone.
        }
        VMData.CurrentCharacterName = character.Name;
    }

    [Command(EnumCommandCategory.Game)]
    public void CurrentWeaponClick(DetectiveInfo weapon) //had to try to change to detectiveinfo to support blazor.
    {
        if (mainGame.OtherTurn > 0)
        {
            return; //because you have to just give a clue alone.
        }
        VMData.CurrentWeaponName = weapon.Name;
    }
    public bool CanStartOver()
    {
        if (gameContainer.SaveRoot.GameStatus == EnumClueStatusList.StartTurn
            || gameContainer.SaveRoot.GameStatus == EnumClueStatusList.FindClues
            || gameContainer.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction
            || gameContainer.SaveRoot.GameStatus == EnumClueStatusList.EndGame)
        {
            return false;
        }
        if (gameContainer.SaveRoot.GameStatus == EnumClueStatusList.EndTurn)
        {
            return gameContainer.CurrentCharacter!.CurrentRoom == 0; //if you made it into the room, then you can't.
        }
        //has to move one space.  or its not even worth doing it.
        if (VMData.Cup!.DiceList.Count == 0)
        {
            return false; //because did not even roll yet.
        }
        if (VMData.LeftToMove == VMData.Cup!.ValueOfOnlyDice)
        {
            return false;
        }
        return true;
        //return gameContainer.SaveRoot.GameStatus == EnumClueStatusList.MoveSpaces;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task StartOverAsync()
    {
        //see if i can figure out when it needs to enable it.

        //VMData.LeftToMove = VMData.Cup!.ValueOfOnlyDice;
        //start out with extending move (will eventually rethink).
        if (gameContainer.BasicData.MultiPlayer)
        {
            await gameContainer.Network!.SendAllAsync("startover");
        }
        await mainGame.StartOverAsync();
    }
    public bool CanMakePrediction
    {
        get
        {
            if (VMData.CurrentWeaponName == "" || VMData.CurrentCharacterName == "")
            {
                return false;
            }
            if (gameContainer.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction)
            {
                return true;
            }
            if (gameContainer.SaveRoot.GameStatus == EnumClueStatusList.StartTurn)
            {
                if (gameContainer.CurrentCharacter!.PreviousRoom != gameContainer.CurrentCharacter.CurrentRoom)
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
        gameContainer!.SaveRoot!.CurrentPrediction!.CharacterName = VMData.CurrentCharacterName;
        gameContainer.SaveRoot.CurrentPrediction.WeaponName = VMData.CurrentWeaponName;
        await mainGame.MakePredictionAsync();
    }
    public bool CanMakeAccusation
    {
        get
        {
            if (gameContainer.SaveRoot.GameStatus == EnumClueStatusList.FindClues)
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

        gameContainer!.SaveRoot!.CurrentPrediction!.CharacterName = VMData.CurrentCharacterName;
        gameContainer.SaveRoot.CurrentPrediction.WeaponName = VMData.CurrentWeaponName;
        gameContainer.SaveRoot.CurrentPrediction.RoomName = VMData.CurrentRoomName;
        await mainGame.MakeAccusationAsync();
    }
    public override bool CanRollDice()
    {
        return gameContainer.SaveRoot.GameStatus == EnumClueStatusList.StartTurn;
    }
    public override bool CanEndTurn()
    {
        return gameContainer.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction || gameContainer.SaveRoot.GameStatus == EnumClueStatusList.EndTurn;
    }
    public override async Task RollDiceAsync()
    {
        await base.RollDiceAsync();
    }
    [Command(EnumCommandCategory.Limited)]
    public void FillInClue(DetectiveInfo detective)
    {
        if (gameContainer is null)
        {
            throw new CustomBasicException("Must have container");
        }
        if (mainGame.OtherTurn > 0)
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
                gameContainer!.SaveRoot!.CurrentPrediction!.CharacterName = "";
                gameContainer.SaveRoot.CurrentPrediction.WeaponName = "";
                gameContainer.SaveRoot.CurrentPrediction.RoomName = "";
                gameContainer.Command.ManuelFinish = false;
                gameContainer.Command.IsExecuting = false;
                gameBoard.RepaintBoard();
                //gameContainer.Command.UpdateAll();
                break;
            default:
                throw new CustomBasicException("Should have chosen yes or no");
        }
    }
}