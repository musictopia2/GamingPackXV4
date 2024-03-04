namespace ClueCardGame.Core.Logic;
[SingletonGame]
public class ClueCardGameMainGameClass
    : CardGameClass<ClueCardGameCardInformation, ClueCardGamePlayerItem, ClueCardGameSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly ClueCardGameVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly ClueCardGameGameContainer _gameContainer; //if we don't need it, take it out.
    private readonly IToast _toast;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    private readonly IMessageBox _message;
#pragma warning disable IDE0290 // Use primary constructor
    public ClueCardGameMainGameClass(IGamePackageResolver mainContainer,
#pragma warning restore IDE0290 // Use primary constructor
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        ClueCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<ClueCardGameCardInformation> cardInfo,
        CommandContainer command,
        ClueCardGameGameContainer gameContainer,
        ISystemError error,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume,
        IMessageBox message
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
        _privateAutoResume = privateAutoResume;
        _message = message;
    }
    public static BasicList<int> ExcludeList { get; set; } = [];


    public int MyID => PlayerList!.GetSelf().Id; //i think.
    public int OtherTurn
    {
        get
        {
            return SaveRoot!.PlayOrder.OtherTurn;
        }
        set
        {
            SaveRoot!.PlayOrder.OtherTurn = value;
        }
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        _model.Accusation.HandList.Clear();
        _model.Accusation.Visible = false;
        _model.Pile1.Visible = true;
        _model.Pile1.ClearCards();
        SingleInfo = PlayerList.GetSelf();
        _model.PlayerHand1.HandList = SingleInfo!.MainHandList;
        bool rets;
        rets = await _privateAutoResume.HasAutoResumeAsync();
        if (rets == false)
        {
            _gameContainer.DetectiveDetails = new();
            _gameContainer.DetectiveDetails.PersonalNotebook = GetDetectiveList();
            await _privateAutoResume.SaveStateAsync(_gameContainer);
        }

        this.ShowTurn();

        //this is private autoresume.


        await base.FinishGetSavedAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true; //i think needs to be here.
    }
    private Dictionary<int, DetectiveInfo> GetDetectiveList()
    {
        Dictionary<int, DetectiveInfo> output = [];
        DetectiveInfo thisD;

        foreach (var card in _gameContainer.DeckList)
        {
            thisD = new();
            thisD.Category = card.WhatType;
            thisD.Name = card.Name;
            thisD.IsChecked = false;
            output.Add(card.Deck, thisD); //so still one based.
        }
        return output;
    }
    protected override async Task LastPartOfSetUpBeforeBindingsAsync()
    {
        //since i am using more private autoresume, the host will store the information privately.
        _gameContainer.DetectiveDetails = new();
        _model.Accusation.Visible = false;
        _model.Accusation.HandList.Clear();
        _model.Prediction.HandList.Clear(); //i think.
        _gameContainer.DetectiveDetails.PersonalNotebook = GetDetectiveList();
        var list = PlayerList.GetAllComputerPlayers();
        foreach (var player in list)
        {
            var item = GetDetectiveList();
            PrivatePlayer fins = new();
            fins.Id = player.Id; //to link up player.
            fins.ComputerDetectiveNoteBook = item;
            _gameContainer.DetectiveDetails.ComputerData.Add(fins);
        }
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    protected override async Task ComputerTurnAsync()
    {
        await EndTurnAsync(); //for now, has to end turn until i make more progress in the game.

        //if there is nothing, then just won't do anything.
        //await Task.CompletedTask;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();

        //at this point, all cards has been used.
        ExcludeList.Clear();
        var list = _gameContainer.DeckList.Where(x => x.WhatType == EnumCardType.IsWeapon).ToBasicList();
        var item = list.GetRandomItem();
        SaveRoot.Solution = new();
        SaveRoot.Solution.WeaponName = item.Name;
        ExcludeList.Add(item.Deck);
        list = _gameContainer.DeckList.Where(x => x.WhatType == EnumCardType.IsCharacter).ToBasicList();
        item = list.GetRandomItem();
        ExcludeList.Add(item.Deck);
        SaveRoot.Solution.CharacterName = item.Name;
        list = _gameContainer.DeckList.Where(x => x.WhatType == EnumCardType.IsRoom).ToBasicList();
        item = list.GetRandomItem();
        ExcludeList.Add(item.Deck);
        SaveRoot.Solution.RoomName = item.Name;
        return base.StartSetUpAsync(isBeginning);
    }

    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.
            case "prediction":
                SaveRoot!.CurrentPrediction = await js1.DeserializeObjectAsync<PredictionInfo>(content);
                _model!.FirstName = SaveRoot.CurrentPrediction.FirstName;
                _model!.SecondName = SaveRoot.CurrentPrediction.SecondName;
                _gameContainer.Command.UpdateAll();
                await MakePredictionAsync();

                return;
            case "accusation":
                SolutionInfo accusation = await js1.DeserializeObjectAsync<SolutionInfo>(content);
                if (status == "prediction")
                {
                    await MakePredictionAsync();
                }
                else
                {
                    await MakeAccusationAsync(accusation);
                }
                return;
            case "cluegiven":
                //needs a new model for cluegiven.

                HintInfo hint = await js1.DeserializeObjectAsync<HintInfo>(content);
                ClueCardGameCardInformation card = new();
                card.Populate(hint.Deck);
                SingleInfo = PlayerList.GetWhoPlayer();
                SaveRoot!.GameStatus = EnumClueStatusList.EndTurn;
                SaveRoot.WhoGaveClue = hint.NickName;
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                {
                    await MarkCardAsync(SingleInfo, card, false); //try this way now (?)
                    _model!.Pile1!.AddCard(card);
                    _command.UpdateAll();
                    //for  now, show a messagebox with the information unless i find another way.
                    //await _message!.ShowMessageAsync($"Clue given by {hint.NickName}"); //hopefully this is okay.
                }
                await ContinueTurnAsync(); //try this (?)
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        SaveRoot.WhoGaveClue = "";

        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.

        //anything else is here.  varies by game.


        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    private void FillOutAccusation(SolutionInfo accusation)
    {
        _model.Accusation.Visible = true;
        //_model.Accusation.HandList.Clear();
        var card = _gameContainer.GetClonedCard(accusation.CharacterName);
        BasicList<ClueCardGameCardInformation> list = [];
        list.Add(card);
        card = _gameContainer.GetClonedCard(accusation.WeaponName);
        list.Add(card);
        card = _gameContainer.GetClonedCard(accusation.RoomName);
        list.Add(card);
        _model.Accusation.HandList.ReplaceRange(list);
    }
    public async Task MakeAccusationAsync(SolutionInfo accusation)
    {
        if (accusation.CharacterName == "" || accusation.WeaponName == "" || accusation.RoomName == "")
        {
            throw new CustomBasicException("The accusation was not filled out");
        }
        _command.ManuelFinish = true;
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self && Test!.NoAnimations == false)
        {
            FillOutAccusation(accusation);
            await Delay!.DelaySeconds(.75);
        }


        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.CanSendMessage(BasicData!))
        {
            await Network!.SendAllAsync("accusation", accusation!);
        }
        if (accusation.CharacterName == SaveRoot.Solution.CharacterName &&
            accusation.RoomName == SaveRoot.Solution.RoomName &&
            accusation.WeaponName == SaveRoot.Solution.WeaponName)
        {
            _toast.ShowInfoToast($"The accusation was correct.  {accusation.CharacterName} did it in the {accusation.RoomName} with the {accusation.WeaponName}");
            await ShowWinAsync();
            return;
        }
        if (WhoTurn == MyID)
        {
            _toast.ShowWarningToast($"Sorry, the accusation was not correct {SingleInfo.NickName}.  You are out of the game but need to still be there in order to prove other predictions wrong.");
        }
        SingleInfo.InGame = false;
        if (PlayerList.Count(items => items.InGame == true) <= 1)
        {
            _toast.ShowWarningToast($"Sorry, nobody got the solution correct.  Therefore, nobody won.  The solution was {Constants.VBCrLf} {accusation.CharacterName} did it in the {accusation.RoomName} with the {accusation.WeaponName}");
            await ShowTieAsync();
            return;
        }
        SaveRoot.GameStatus = EnumClueStatusList.EndTurn;
        await EndStepAsync();
    }
    private void FillOutPrediction()
    {
        var card = _gameContainer.GetClonedCard(SaveRoot.CurrentPrediction!.FirstName);
        BasicList<ClueCardGameCardInformation> list = [];
        list.Add(card);
        card = _gameContainer.GetClonedCard(SaveRoot.CurrentPrediction!.SecondName);
        list.Add(card);
        _model.Prediction.HandList.ReplaceRange(list);
    }
    public async Task MakePredictionAsync()
    {
        _command.ManuelFinish = true;
        OtherTurn = 0;
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self && Test!.NoAnimations == false)
        {
            FillOutPrediction();
            await Delay!.DelaySeconds(.75);
        }
        if (SingleInfo.CanSendMessage(BasicData!))
        {
            await Network!.SendAllAsync("prediction", SaveRoot.CurrentPrediction);
        }
        int x = 0;
        do
        {
            x++;
            OtherTurn = await PlayerList.CalculateOtherTurnAsync();
            if (x > 10)
            {
                throw new CustomBasicException("Too Much");
            }
            if (OtherTurn == 0)
            {
                break;
            }
            if (CanGiveCard())
            {
                break;
            }
        } while (true);
        if (OtherTurn == 0)
        {
            SingleInfo = PlayerList.GetWhoPlayer();
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
            {
                await ComputerNoCluesFoundAsync();
            }
            SaveRoot.GameStatus = EnumClueStatusList.EndTurn; //very annoying to show no clues given each time.
            await EndStepAsync();
            return;
        }
        SaveRoot.GameStatus = EnumClueStatusList.FindClues;
        await EndStepAsync();
    }
    private bool CanGiveCard()
    {
        if (SaveRoot!.CurrentPrediction!.FirstName == "" || SaveRoot.CurrentPrediction.SecondName == "")
        {
            throw new CustomBasicException("Cannot use the CanGiveCard function because the prediction was not filled out completely");
        }
        var tempPlayer = PlayerList.GetOtherPlayer();
        if (tempPlayer.MainHandList.Any(x => x.Name == SaveRoot.CurrentPrediction.FirstName))
        {
            return true;
        }
        if (tempPlayer.MainHandList.Any(x => x.Name == SaveRoot.CurrentPrediction.SecondName))
        {
            return true;
        }
        return false;
    }

    private async Task EndStepAsync()
    {
        await SaveStateAsync(); //i tihnk needs to be here now.

        if (SaveRoot!.GameStatus == EnumClueStatusList.EndTurn)
        {
            OtherTurn = 0;
        }
        if (OtherTurn == 0 && WhoTurn == MyID)
        {
            await ShowHumanCanPlayAsync();
            return;
        }
        if (OtherTurn > 0 && OtherTurn == MyID)
        {
            var player = PlayerList.GetWhoPlayer();
            await _message.ShowMessageAsync($"You need to give a clue for the {player.NickName}");
            await ShowHumanCanPlayAsync();
            return;
        }
        if (BasicData!.MultiPlayer == false)
        {
            await ComputerRegularTurnAsync();
            return;
        }
        if (BasicData.MultiPlayer == true && OtherTurn == 0)
        {
            Network!.IsEnabled = true;
            return;
        }
        if (OtherTurn > 0)
        {
            SingleInfo = PlayerList!.GetOtherPlayer(); //i think.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
            {
                if (BasicData.MultiPlayer == true && BasicData.Client)
                {
                    Network!.IsEnabled = true;
                    return;
                }
                await ComputerRegularTurnAsync();
                return;
            }
            if (BasicData.MultiPlayer)
            {
                Network!.IsEnabled = true;
                return;
            }
        }
    }
    private string CardToGive()
    {
        var player = _gameContainer.DetectiveDetails!.ComputerData.Single(x => x.Id == OtherTurn);
        GivenInfo? card = player.CluesGiven.Where(x => x.Player == WhoTurn &&
            (x.Clue == SaveRoot.CurrentPrediction!.FirstName
            || x.Clue == SaveRoot.CurrentPrediction.SecondName)).FirstOrDefault();

        if (card is not null)
        {
            return card.Clue;
        }
        ClueCardGamePlayerItem tempPlayer = PlayerList!.GetOtherPlayer();
        BasicList<GivenInfo> gives = [];
        tempPlayer.MainHandList.ForEach(thisCard =>
        {
            if (thisCard.Name == SaveRoot!.CurrentPrediction!.FirstName
            || thisCard.Name == SaveRoot.CurrentPrediction.SecondName)
            {
                GivenInfo newGiven = new();
                newGiven.Player = WhoTurn;
                newGiven.Clue = thisCard.Name;
                gives.Add(newGiven);
            }
        });
        if (gives.Count == 0)
        {
            throw new CustomBasicException("There was no card to give even though the CardGive function ran");
        }
        var finCard = gives.GetRandomItem();
        player.CluesGiven.Add(finCard);
        return finCard.Clue;
    }
    private void ComputerNearEnd(Dictionary<int, DetectiveInfo> complete, string name)
    {
        //means not bluffing.
        var card = _gameContainer.DeckList.Single(x => x.Name == name);
        var category = card.WhatType;
        var list = _gameContainer.DeckList.Where(x => x.WhatType == category && x.Name != name);
        foreach (var item in list)
        {
            var lasts = complete.Single(x => x.Value.Name == item.Name).Value;
            lasts.IsChecked = true; //can't be this one anymore.
        }
    }
    private async Task ComputerNoCluesFoundAsync()
    {
        var computer = _gameContainer.DetectiveDetails!.ComputerData.Single(x => x.Id == WhoTurn);
        var notes = computer.ComputerDetectiveNoteBook;
        var first = notes.Single(x => x.Value.Name == SaveRoot.CurrentPrediction!.FirstName).Value;
        var second = notes.Single(x => x.Value.Name == SaveRoot.CurrentPrediction!.SecondName).Value;
        if (first.WasGiven == false)
        {
            ComputerNearEnd(notes, first.Name);
        }
        if (second.WasGiven == false)
        {
            ComputerNearEnd(notes, second.Name);
        }
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }

    private async Task ComputerRegularTurnAsync()
    {
        if (OtherTurn > 0 && BasicData!.MultiPlayer && BasicData.Client)
        {
            return;
        }
        if (SaveRoot!.GameStatus == EnumClueStatusList.MakePrediction)
        {
            await EndTurnAsync();
            return; //the computer has to skip their turns because it was really hosed.
        }
        if (SaveRoot.GameStatus == EnumClueStatusList.FindClues)
        {
            string thisInfo = CardToGive();
            ClueCardGamePlayerItem newPlayer = PlayerList!.GetOtherPlayer();
            ClueCardGameCardInformation thisCard = newPlayer.MainHandList.Single(items => items.Name == thisInfo);
            SingleInfo = PlayerList.GetWhoPlayer();

            if (newPlayer.CanSendMessage(BasicData!) && WhoTurn != MyID)
            {
                HintInfo hint = new()
                {
                    Deck = thisCard.Deck,
                    NickName = newPlayer.NickName
                };
                await Network!.SendAllAsync("cluegiven", hint); //has to send to all so i can eventually have autoresume.
                //await Network!.SendToParticularPlayerAsync("cluegiven", thisCard.Deck, SingleInfo.NickName);
            }
            SaveRoot.GameStatus = EnumClueStatusList.EndTurn;
            if (WhoTurn == MyID)
            {
                _model.Pile1.AddCard(thisCard);
            }
            await MarkCardAsync(SingleInfo, thisCard, false);
            SaveRoot.WhoGaveClue = newPlayer.NickName;
            _command.UpdateAll();
            await EndStepAsync();
            return;
        }
        throw new CustomBasicException("The computer should have skipped their turns since their moving was really hosed");
    }
    public async Task MarkCardAsync(ClueCardGamePlayerItem player, ClueCardGameCardInformation card, bool beginning)
    {
        if (_gameContainer is null)
        {
            throw new CustomBasicException("Must have game container"); //did this so does not expect me to use static.
        }
        if (beginning == false)
        {
            SaveRoot.PreviousClue = card.Deck; //i think
        }
        if (player.PlayerCategory == EnumPlayerCategory.Self)
        {
            var item = _gameContainer.DetectiveDetails!.PersonalNotebook.Single(x => x.Value.Name == card.Name).Value;
            //will check automatically.  but also show it was given (even if its beginning).
            item.WasGiven = true;
            item.IsChecked = true;
            await _privateAutoResume.SaveStateAsync(_gameContainer);
            return;
        }
        if (player.PlayerCategory == EnumPlayerCategory.Computer)
        {
            var computer = _gameContainer.DetectiveDetails!.ComputerData.Single(x => x.Id == player.Id);
            var item = computer.ComputerDetectiveNoteBook.Single(x => x.Value.Name == card.Name).Value;
            item.WasGiven = true;
            item.IsChecked = true;
            await _privateAutoResume.SaveStateAsync(_gameContainer);
            return;
        }
    }
}