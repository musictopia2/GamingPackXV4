namespace ClueCardGame.Core.ViewModels;
[InstanceGame]
public partial class ClueCardGameMainViewModel : BasicCardGamesVM<ClueCardGameCardInformation>
{
    private readonly ClueCardGameMainGameClass _mainGame; //if we don't need, delete.
    private readonly IToast _toast;
    private readonly ClueCardGameGameContainer _gameContainer;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    public ClueCardGameVMData VMData { get; set; }
    public ClueCardGameMainViewModel(CommandContainer commandContainer,
        ClueCardGameMainGameClass mainGame,
        ClueCardGameVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast,
        ClueCardGameGameContainer gameContainer,
        PrivateAutoResumeProcesses privateAutoResume
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _toast = toast;
        _gameContainer = gameContainer;
        _privateAutoResume = privateAutoResume;
        VMData.Prediction.SendEnableProcesses(this, () => _mainGame.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction);
        VMData.Prediction.ObjectClickedAsync = PredictionChangeMindAsync;
        VMData.Accusation.ObjectClickedAsync = AccusationChangeMindAsync;
        VMData.PlayerHand1.SendEnableProcesses(this, () => _mainGame.SaveRoot.GameStatus == EnumClueStatusList.FindClues);
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private async Task AccusationChangeMindAsync(ClueCardGameCardInformation card, int index)
    {
        bool rets = await RemoveAccusationAsync(card);
        if (rets == false)
        {
            _toast.ShowUserErrorToast("I think there was a bug because did not find a card to remove for accusation");
        }
    }
    private async Task<bool> RemoveAccusationAsync(ClueCardGameCardInformation card)
    {
        var finds = VMData.Accusation.HandList.SingleOrDefault(x => x.Name == card.Name);
        if (finds is not null)
        {
            VMData.Accusation.HandList.RemoveObjectByDeck(card.Deck);
            if (finds.Name == _gameContainer.DetectiveDetails!.Accusation.RoomName)
            {
                _gameContainer.DetectiveDetails.Accusation.RoomName = "";
            }
            else if (finds.Name == _gameContainer.DetectiveDetails!.Accusation.WeaponName)
            {
                _gameContainer.DetectiveDetails.Accusation.WeaponName = "";
            }
            else if (finds.Name == _gameContainer.DetectiveDetails!.Accusation.RoomName)
            {
                _gameContainer.DetectiveDetails.Accusation.RoomName = "";
            }
            else
            {
                throw new CustomBasicException("Failed to find in the accusation details");
            }
            await _privateAutoResume.SaveStateAsync(_gameContainer);
            return true;
        }
        return false;
    }
    private async Task PredictionChangeMindAsync(ClueCardGameCardInformation card, int index)
    {
        bool rets = await RemovePredictionAsync(card);
        if (rets == false)
        {
            _toast.ShowUserErrorToast("I think there was a bug because did not find a card to remove for prediction");
        }
    }
    private async Task<bool> RemovePredictionAsync(ClueCardGameCardInformation card)
    {
        if (card.Name == _mainGame.SaveRoot.CurrentPrediction!.FirstName)
        {
            _mainGame.SaveRoot.CurrentPrediction.FirstName = "";
            _gameContainer.DetectiveDetails!.CurrentPrediction!.FirstName = "";
            VMData.FirstName = "";
            VMData.Prediction.HandList.RemoveObjectByDeck(card.Deck);
            await _privateAutoResume.SaveStateAsync(_gameContainer);
            return true;
        }
        if (card.Name == _mainGame.SaveRoot.CurrentPrediction.SecondName)
        {
            _mainGame.SaveRoot.CurrentPrediction.SecondName = "";
            _gameContainer.DetectiveDetails!.CurrentPrediction!.SecondName = "";
            VMData.SecondName = "";
            VMData.Prediction.HandList.RemoveObjectByDeck(card.Deck);
            await _privateAutoResume.SaveStateAsync(_gameContainer);
            return true;
        }
        return false;
    }
    protected override bool CanEnableDeck()
    {
        return false; //otherwise, can't compile.
    }
    protected override bool CanEnablePile1()
    {
        return false; //otherwise, can't compile.
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return false;
    }
    public override bool CanEndTurn() => _mainGame.SaveRoot.GameStatus == EnumClueStatusList.EndTurn && _gameContainer.DetectiveDetails!.StartAccusation == false;
    public bool CanStartAccusation() => _mainGame.SaveRoot.GameStatus != EnumClueStatusList.FindClues && _gameContainer.DetectiveDetails!.StartAccusation == false && _gameContainer.DetectiveDetails!.HumanFailed == false;
    [Command(EnumCommandCategory.Game)]
    public async Task StartAccusationAsync()
    {
        _gameContainer.DetectiveDetails!.StartAccusation = true;
        _gameContainer.DetectiveDetails.Accusation = new();
        VMData.Accusation.ClearHand();
        VMData.Prediction.ClearHand(); //i think will clear hand here too.
        VMData.Accusation.Visible = true;
        VMData.Prediction.Visible = false;
        _mainGame.SaveRoot.CurrentPrediction!.FirstName = "";
        _mainGame.SaveRoot.CurrentPrediction.SecondName = "";
        _mainGame.SaveRoot.LoadMod(VMData);
        _gameContainer.DetectiveDetails.CurrentPrediction!.FirstName = "";
        _gameContainer.DetectiveDetails.CurrentPrediction.SecondName = "";
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    public bool CanCancelAccusation => _gameContainer.DetectiveDetails!.StartAccusation;
    [Command(EnumCommandCategory.Game)]
    public async Task CancelAccusationAsync()
    {
        _gameContainer.DetectiveDetails!.StartAccusation = false;
        _gameContainer.DetectiveDetails.Accusation = new();
        VMData.Accusation.Visible = false;
        VMData.Prediction.Visible = true;
        VMData.Accusation.ClearHand(); //has to clear hand because you cancelled.  if you do again, redo.
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    public bool CanAddAccusation => _gameContainer.DetectiveDetails!.StartAccusation && _mainGame.SaveRoot.GameStatus != EnumClueStatusList.FindClues;
    [Command(EnumCommandCategory.Game)]
    public async Task AddAccusationAsync(ClueCardGameCardInformation card)
    {
        bool rets;
        rets = await RemoveAccusationAsync(card);
        if (rets)
        {
            return;
        }
        ClueCardGameCardInformation newItem;
        newItem = new();
        newItem.Populate(card.Deck);
        //can replace easily as well if you really want to.

        //this means if you pick 2 rooms, one will replace the other.

        var fins = VMData.Accusation.HandList.SingleOrDefault(x => x.WhatType == card.WhatType);
        if (fins is null)
        {
            VMData.Accusation.HandList.Add(newItem);
        }
        else
        {
            VMData.Accusation.HandList.ReplaceItem(fins, newItem); //hopefully this works (?)
        }
        if (card.WhatType == EnumCardType.IsWeapon)
        {
            _gameContainer.DetectiveDetails!.Accusation.WeaponName = card.Name;
        }
        else if (card.WhatType == EnumCardType.IsRoom)
        {
            _gameContainer.DetectiveDetails!.Accusation.RoomName = card.Name;
        }
        else if (card.WhatType == EnumCardType.IsCharacter)
        {
            _gameContainer.DetectiveDetails!.Accusation.CharacterName = card.Name;
        }
        else
        {
            throw new CustomBasicException("Failed to add accusation");
        }
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    public bool CanAddPrediction => _mainGame.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction && _gameContainer.DetectiveDetails!.StartAccusation == false;
    [Command(EnumCommandCategory.Game)]
    public async Task AddPredictionAsync(ClueCardGameCardInformation card)
    {
        bool rets;
        rets = await RemovePredictionAsync(card);
        if (rets)
        {
            return;
        }
        ClueCardGameCardInformation newItem;
        if (VMData.FirstName == "")
        {
            //replace it.
            _mainGame.SaveRoot.CurrentPrediction!.FirstName = card.Name;
            _gameContainer.DetectiveDetails!.CurrentPrediction!.FirstName = card.Name;
            VMData.FirstName = card.Name;
            newItem = new();
            newItem.Populate(card.Deck);
            VMData.Prediction.HandList.Add(newItem);
            await _privateAutoResume.SaveStateAsync(_gameContainer);
            return;
        }
        if (VMData.SecondName == "")
        {
            _mainGame.SaveRoot.CurrentPrediction!.SecondName = card.Name;
            _gameContainer.DetectiveDetails!.CurrentPrediction!.SecondName = card.Name;
            VMData.SecondName = card.Name;
            newItem = new();
            newItem.Populate(card.Deck);
            VMData.Prediction.HandList.Add(newItem);
            await _privateAutoResume.SaveStateAsync(_gameContainer);
            return;
        }
        _toast.ShowUserErrorToast("You already chose the 2 cards.  Click on a card in the prediction area to remove it");
        //_toast.ShowInfoToast(card.Deck.ToString());
        //await Task.Delay(1);
    }
    public bool CanMakePrediction
    {
        get
        {
            if (VMData.FirstName == "" || VMData.SecondName == "")
            {
                return false;
            }
            if (VMData.FirstName == VMData.SecondName)
            {
                return false;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction)
            {
                return true;
            }

            return false;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakePredictionAsync()
    {
        _mainGame!.SaveRoot!.CurrentPrediction!.FirstName = VMData.FirstName;
        _mainGame.SaveRoot.CurrentPrediction.SecondName = VMData.SecondName;
        await _mainGame.MakePredictionAsync();
    }
    public bool CanMakeAccusation()
    {
        if (_mainGame.SaveRoot.GameStatus == EnumClueStatusList.FindClues)
        {
            return false;
        }
        if (_gameContainer.DetectiveDetails!.Accusation.CharacterName == "")
        {
            return false;
        }
        if (_gameContainer.DetectiveDetails!.Accusation.WeaponName == "")
        {
            return false;
        }
        if (_gameContainer.DetectiveDetails!.Accusation.RoomName == "")
        {
            return false;
        }
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeAccusationAsync()
    {
        //this means you can make it because all is filled out.
        await _mainGame.MakeAccusationAsync(_gameContainer.DetectiveDetails!.Accusation);
    }
    protected override async Task ProcessHandClickedAsync(ClueCardGameCardInformation card, int index)
    {
        if (_gameContainer.CanGiveCard(card) == false)
        {
            _toast.ShowUserErrorToast("Unable to give card as clue");
            return;
        }
        card.IsSelected = true;
        CommandContainer.UpdateAll(); //to notify blazor.
        if (_gameContainer.Test.NoAnimations == false)
        {
            await _gameContainer.Delay.DelaySeconds(.25);
        }
        var player = _gameContainer.PlayerList!.GetSelf();
        var tempPlayer = _gameContainer!.PlayerList!.GetWhoPlayer();
        if (_gameContainer.BasicData!.MultiPlayer)
        {
            HintInfo hint = new()
            {
                Deck = card.Deck,
                NickName = player.NickName
            };
            await _gameContainer.Network!.SendAllAsync("cluegiven", hint);
            //only whoever turn it is needs to show who gave it.
            //has to send to everybody even though there is private autoresume.  that way can be recorded for everybody (for additional clues)
        }
        CommandContainer!.ManuelFinish = true;
        card.IsSelected = false;
        await _mainGame.MarkCardAsync(tempPlayer, card, false);
        _gameContainer.SaveRoot!.GameStatus = EnumClueStatusList.EndTurn;
        if (_gameContainer.BasicData.MultiPlayer == false)
        {
            throw new CustomBasicException("Computer should have never had this");
        }
        _gameContainer.Network!.IsEnabled = true; //to wait for them to end turn.
    }
}