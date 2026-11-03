namespace MonasteryCardGame.Core.ViewModels;
[InstanceGame]
public partial class MonasteryCardGameMainViewModel(CommandContainer commandContainer,
        MonasteryCardGameMainGameClass mainGame,
        MonasteryCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        MonasteryCardGameGameContainer gameContainer,
        IToast toast,
        IMessageBox message,
        PrivateAutoResumeProcesses privateAutoResume) : BasicCardGamesVM<MonasteryCardInfo>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        var player = mainGame.PlayerList.GetSelf();
        player.DoInit();
        viewModel.TempSets.Init(this);
        viewModel.TempSets.ClearBoard();
        PossibleAutoResume();
        viewModel.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        viewModel.MainSets.SetClickedAsync = MainSets_SetClickedAsync;
        viewModel.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
        viewModel.MainSets.SendEnableProcesses(this, () =>
        {
            if (gameContainer!.AlreadyDrew == false)
            {
                return false;
            }
            return mainGame.SingleInfo!.FinishedCurrentMission;
        });
        CreateCommands();
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands();
    partial void CreateCommands(CommandContainer command);
    private void PossibleAutoResume()
    {
        if (gameContainer.TempSets.Count > 0)
        {
            var player = gameContainer.PlayerList!.GetSelf();
            bool hadAny = false;
            foreach (var item in gameContainer.TempSets)
            {
                var current = viewModel.TempSets.SetList[item.SetNumber - 1];
                var cards = item.Cards.GetNewObjectListFromDeckList(gameContainer.DeckList);
                DeckRegularDict<MonasteryCardInfo> toAdd = [];
                foreach (var card in cards)
                {
                    if (player.MainHandList.ObjectExist(card.Deck))
                    {
                        player.MainHandList.RemoveObjectByDeck(card.Deck);
                        player.AdditionalCards.Add(card); //if i remove from hand, must add to additional cards so sends to other players properly.
                        hadAny = true;
                        toAdd.Add(card);
                    }
                }
                current.AddCards(toAdd);
            }
            if (hadAny)
            {
                viewModel.TempSets.PublicCount();
            }
        }
    }
    private bool _isProcessing;
    private async Task TempSets_SetClickedAsync(int index)
    {
        if (_isProcessing == true)
        {
            return;
        }
        _isProcessing = true;
        var tempList = viewModel.PlayerHand1!.ListSelectedObjects(true);
        viewModel.TempSets!.AddCards(index, tempList);
        await privateAutoResume.SaveStateAsync(viewModel);
        _isProcessing = false;
    }
    private DeckRegularDict<MonasteryCardInfo> GetSelectCards()
    {
        var firstList = viewModel.PlayerHand1!.ListSelectedObjects();
        var newCol = viewModel.TempSets!.ListSelectedObjects();
        DeckRegularDict<MonasteryCardInfo> output = [];
        output.AddRange(firstList);
        output.AddRange(newCol);
        return output;
    }
    protected override bool CanEnableDeck()
    {
        return !gameContainer.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        if (mainGame!.CanProcessDiscard(out bool pickUp, out _, out int deck, out string message) == false)
        {
            Toast.ShowUserErrorToast(message);
            return;
        }
        if (pickUp == true)
        {
            await mainGame.PickupFromDiscardAsync();
            return;
        }
        await gameContainer.SendDiscardMessageAsync(deck);
        await mainGame.DiscardAsync(deck);
    }
    public string MissionChosen => viewModel.MissionChosen;
    public override bool CanEnableAlways()
    {
        return true;
    }
    private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
    {
        if (setNumber == 0)
        {
            Toast.ShowUserErrorToast("You must click on an existing set in order to expand.");
            return;
        }
        if (mainGame!.SingleInfo!.FinishedCurrentMission == false)
        {
            throw new CustomBasicException("Should have been disabled because you did not finish the mission");
        }
        var thisCol = GetSelectCards();
        if (thisCol.Count > 2)
        {
            Toast.ShowUserErrorToast("You cannot select more than 2 cards");
            return;
        }
        if (thisCol.Count == 0)
        {
            Toast.ShowUserErrorToast("There are no cards selected");
            return;
        }
        var thisSet = viewModel.MainSets!.GetIndividualSet(setNumber);
        bool doubles = thisSet.IsDoubleRun;
        if (doubles && thisCol.Count < 2)
        {
            Toast.ShowUserErrorToast("There needs to be 2 cards selected because this is a double run of three");
            return;
        }
        if (doubles == false && thisCol.Count > 1)
        {
            Toast.ShowUserErrorToast("Since its not a double run of three, then can only choose one card at a time");
            return;
        }
        int newpos = 0;
        foreach (var thisCard in thisCol)
        {
            newpos = thisSet.PositionToPlay(thisCard, section);
            if (newpos == 0)
            {
                Toast.ShowUserErrorToast("Sorry, cannot use the card to expand upon");
                return;
            }
        }
        int x = 0;
        int nums = 0;
        foreach (var thisRummy in viewModel.MainSets.SetList)
        {
            x++;
            if (thisRummy.Equals(thisSet))
            {
                nums = x;
                break;
            }
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            SendExpandSet temps = new();
            temps.SetNumber = nums;
            temps.Position = newpos;
            temps.CardData = await js1.SerializeObjectAsync(thisCol.GetDeckListFromObjectList());
            await mainGame.Network!.SendAllAsync("expandset", temps);
        }
        await mainGame.ExpandSetAsync(thisCol, nums, newpos);
    }
    [Command(EnumCommandCategory.Old)]
    public async Task MissionDetailAsync()
    {
        if (viewModel is null)
        {
            //code.
        }
        await message.ShowMessageAsync("Mission 1:  2 sets of 3 in color" + Constants.VBCrLf + "Mission 2:  3 sets of 3" + Constants.VBCrLf + "Mission 3:  1 set of 4, 1 run or 4" + Constants.VBCrLf + "Mission 4:  1 run of 5 in suit" + Constants.VBCrLf + "Mission 5:  1 run of 6 in color" + Constants.VBCrLf + "Mission 6:  1 run of 8: " + Constants.VBCrLf + "Mission 7:  1 double run of 3" + Constants.VBCrLf + "Mission 8:  7 cards of the same suit" + Constants.VBCrLf + "Mission 9:  9 cards of even rank (2, 4, 6, 8, 10, 12) or 9 cards of odd rank (1, 3, 5, 7, 9, 11, 13)");
    }
    public bool CanSelectPossibleMission()
    {
        if (!gameContainer!.AlreadyDrew)
        {
            return false; //has to draw first.
        }
        return !mainGame.SingleInfo!.FinishedCurrentMission;
    }
    [Command(EnumCommandCategory.Game)]
    public void SelectPossibleMission(MissionList mission)
    {
        viewModel.MissionChosen = mission.Description;
    }
    public bool CanCompleteChosenMission()
    {
        if (mainGame!.SingleInfo!.FinishedCurrentMission)
        {
            return false;
        }
        return string.IsNullOrEmpty(viewModel.MissionChosen) == false;
    }

    [Command(EnumCommandCategory.Game)]
    public async Task CompleteChosenMissionAsync()
    {
        bool rets = mainGame!.DidCompleteMission(out BasicList<InstructionInfo> tempList);
        if (rets == false)
        {
            Toast.ShowUserErrorToast("Sorry, you did not complete the mission");
            viewModel.MissionChosen = "";
            return;
        }
        mainGame.ProcessCurrentMission();
        BasicList<string> mList = [];
        await tempList.ForEachAsync(async thisTemp =>
        {
            var thisCol = viewModel.TempSets.ObjectList(thisTemp.SetNumber).ToRegularDeckDict();
            if (thisCol.Count == 0)
            {
                throw new CustomBasicException("Cannot have 0 items");
            }
            if (mainGame.BasicData!.MultiPlayer)
            {
                SendNewSet thisSend = new();
                thisSend.Index = thisTemp.WhichOne;
                var newCol = thisCol.GetDeckListFromObjectList();
                thisSend.CardData = await js1.SerializeObjectAsync(newCol);
                thisSend.MissionCompleted = viewModel.MissionChosen;
                string results = await js1.SerializeObjectAsync(thisSend);
                mList.Add(results);
            }
            viewModel.TempSets.ClearBoard(thisTemp.SetNumber);
            mainGame.CreateNewSet(thisCol, thisTemp.WhichOne);
        });
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendSeveralSetsAsync(mList, "finished");
        }
        await mainGame.FinishedAsync();
    }
}