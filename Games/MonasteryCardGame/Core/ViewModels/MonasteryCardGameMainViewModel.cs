namespace MonasteryCardGame.Core.ViewModels;
[InstanceGame]
public partial class MonasteryCardGameMainViewModel : BasicCardGamesVM<MonasteryCardInfo>
{
    private readonly MonasteryCardGameMainGameClass _mainGame;
    private readonly MonasteryCardGameVMData _model;
    private readonly MonasteryCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly IMessageBox _message;
    public MonasteryCardGameMainViewModel(CommandContainer commandContainer,
        MonasteryCardGameMainGameClass mainGame,
        MonasteryCardGameVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        MonasteryCardGameGameContainer gameContainer,
        IToast toast,
        IMessageBox message
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _gameContainer = gameContainer;
        _toast = toast;
        _message = message;
        _model.Deck1.NeverAutoDisable = true;
        var player = _mainGame.PlayerList.GetSelf();
        player.DoInit();
        _model.TempSets.Init(this);
        _model.TempSets.ClearBoard();
        _model.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        _model.MainSets.SetClickedAsync = MainSets_SetClickedAsync;
        _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
        _model.MainSets.SendEnableProcesses(this, () =>
        {
            if (_gameContainer!.AlreadyDrew == false)
            {
                return false;
            }
            return _mainGame.SingleInfo!.FinishedCurrentMission;
        });
        CreateCommands();
        CreateCommands(commandContainer);
    }
    partial void CreateCommands();
    partial void CreateCommands(CommandContainer command);
    private bool _isProcessing;
    private Task TempSets_SetClickedAsync(int index)
    {
        if (_isProcessing == true)
        {
            return Task.CompletedTask;
        }
        _isProcessing = true;
        var tempList = _model.PlayerHand1!.ListSelectedObjects(true);
        _model.TempSets!.AddCards(index, tempList);
        _isProcessing = false;
        return Task.CompletedTask;
    }
    private DeckRegularDict<MonasteryCardInfo> GetSelectCards()
    {
        var firstList = _model.PlayerHand1!.ListSelectedObjects();
        var newCol = _model.TempSets!.ListSelectedObjects();
        DeckRegularDict<MonasteryCardInfo> output = new();
        output.AddRange(firstList);
        output.AddRange(newCol);
        return output;
    }
    protected override bool CanEnableDeck()
    {
        return !_gameContainer.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        if (_mainGame!.CanProcessDiscard(out bool pickUp, out _, out int deck, out string message) == false)
        {
            _toast.ShowUserErrorToast(message);
            return;
        }
        if (pickUp == true)
        {
            await _mainGame.PickupFromDiscardAsync();
            return;
        }
        await _gameContainer.SendDiscardMessageAsync(deck);
        await _mainGame.DiscardAsync(deck);
    }
    public string MissionChosen => _model.MissionChosen;
    public override bool CanEnableAlways()
    {
        return true;
    }
    private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
    {
        if (setNumber == 0)
        {
            _toast.ShowUserErrorToast("You must click on an existing set in order to expand.");
            return;
        }
        if (_mainGame!.SingleInfo!.FinishedCurrentMission == false)
        {
            throw new CustomBasicException("Should have been disabled because you did not finish the mission");
        }
        var thisCol = GetSelectCards();
        if (thisCol.Count > 2)
        {
            _toast.ShowUserErrorToast("You cannot select more than 2 cards");
            return;
        }
        if (thisCol.Count == 0)
        {
            _toast.ShowUserErrorToast("There are no cards selected");
            return;
        }
        var thisSet = _model.MainSets!.GetIndividualSet(setNumber);
        bool doubles = thisSet.IsDoubleRun;
        if (doubles && thisCol.Count < 2)
        {
            _toast.ShowUserErrorToast("There needs to be 2 cards selected because this is a double run of three");
            return;
        }
        if (doubles == false && thisCol.Count > 1)
        {
            _toast.ShowUserErrorToast("Since its not a double run of three, then can only choose one card at a time");
            return;
        }
        int newpos = 0;
        foreach (var thisCard in thisCol)
        {
            newpos = thisSet.PositionToPlay(thisCard, section);
            if (newpos == 0)
            {
                _toast.ShowUserErrorToast("Sorry, cannot use the card to expand upon");
                return;
            }
        }
        int x = 0;
        int nums = 0;
        foreach (var thisRummy in _model.MainSets.SetList)
        {
            x++;
            if (thisRummy.Equals(thisSet))
            {
                nums = x;
                break;
            }
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            SendExpandSet temps = new();
            temps.SetNumber = nums;
            temps.Position = newpos;
            temps.CardData = await js1.SerializeObjectAsync(thisCol.GetDeckListFromObjectList());
            await _mainGame.Network!.SendAllAsync("expandset", temps);
        }
        await _mainGame.ExpandSetAsync(thisCol, nums, newpos);
    }
    [Command(EnumCommandCategory.Old)]
    public async Task MissionDetailAsync()
    {
        if (_model is null)
        {
            //code.
        }
        await _message.ShowMessageAsync("Mission 1:  2 sets of 3 in color" + Constants.VBCrLf + "Mission 2:  3 sets of 3" + Constants.VBCrLf + "Mission 3:  1 set of 4, 1 run or 4" + Constants.VBCrLf + "Mission 4:  1 run of 5 in suit" + Constants.VBCrLf + "Mission 5:  1 run of 6 in color" + Constants.VBCrLf + "Mission 6:  1 run of 8: " + Constants.VBCrLf + "Mission 7:  1 double run of 3" + Constants.VBCrLf + "Mission 8:  7 cards of the same suit" + Constants.VBCrLf + "Mission 9:  9 cards of even rank (2, 4, 6, 8, 10, 12) or 9 cards of odd rank (1, 3, 5, 7, 9, 11, 13)");
    }
    public bool CanSelectPossibleMission()
    {
        if (!_gameContainer!.AlreadyDrew)
        {
            return false; //has to draw first.
        }
        return !_mainGame.SingleInfo!.FinishedCurrentMission;
    }
    [Command(EnumCommandCategory.Game)]
    public void SelectPossibleMission(MissionList mission)
    {
        _model.MissionChosen = mission.Description;
    }
    public bool CanCompleteChosenMission()
    {
        if (_mainGame!.SingleInfo!.FinishedCurrentMission)
        {
            return false;
        }
        return string.IsNullOrEmpty(_model.MissionChosen) == false;
    }

    [Command(EnumCommandCategory.Game)]
    public async Task CompleteChosenMissionAsync()
    {
        bool rets = _mainGame!.DidCompleteMission(out BasicList<InstructionInfo> tempList);
        if (rets == false)
        {
            _toast.ShowUserErrorToast("Sorry, you did not complete the mission");
            _model.MissionChosen = "";
            return;
        }
        _mainGame.ProcessCurrentMission();
        BasicList<string> mList = new();
        await tempList.ForEachAsync(async thisTemp =>
        {
            var thisCol = _model.TempSets.ObjectList(thisTemp.SetNumber).ToRegularDeckDict();
            if (thisCol.Count == 0)
            {
                throw new CustomBasicException("Cannot have 0 items");
            }
            if (_mainGame.BasicData!.MultiPlayer)
            {
                SendNewSet thisSend = new();
                thisSend.Index = thisTemp.WhichOne;
                var newCol = thisCol.GetDeckListFromObjectList();
                thisSend.CardData = await js1.SerializeObjectAsync(newCol);
                thisSend.MissionCompleted = _model.MissionChosen;
                string results = await js1.SerializeObjectAsync(thisSend);
                mList.Add(results);
            }
            _model.TempSets.ClearBoard(thisTemp.SetNumber);
            _mainGame.CreateNewSet(thisCol, thisTemp.WhichOne);
        });
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendSeveralSetsAsync(mList, "finished");
        }
        await _mainGame.FinishedAsync();
    }
}