namespace ClueCardGame.Core.ViewModels;
[InstanceGame]
public partial class ClueCardGameMainViewModel(CommandContainer commandContainer,
        ClueCardGameMainGameClass mainGame,
        ClueCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast,
        ClueCardGameGameContainer gameContainer,
        PrivateAutoResumeProcesses privateAutoResume) : BasicCardGamesVM<ClueCardGameCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    public ClueCardGameVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        VMData.Prediction.SendEnableProcesses(this, () => mainGame.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction);
        VMData.Prediction.ObjectClickedAsync = PredictionChangeMindAsync;
        VMData.Accusation.ObjectClickedAsync = AccusationChangeMindAsync;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    protected override bool AlwaysEnableHand()
    {
        return false;
    }
    protected override bool CanEnableHand()
    {
        return mainGame.SaveRoot.GameStatus == EnumClueStatusList.FindClues;
    }
    partial void CreateCommands(CommandContainer command);
    private async Task AccusationChangeMindAsync(ClueCardGameCardInformation card, int index)
    {
        bool rets = await RemoveAccusationAsync(card);
        if (rets == false)
        {
            Toast.ShowUserErrorToast("I think there was a bug because did not find a card to remove for accusation");
        }
    }
    private async Task<bool> RemoveAccusationAsync(ClueCardGameCardInformation card)
    {
        var finds = VMData.Accusation.HandList.SingleOrDefault(x => x.Name == card.Name);
        if (finds is not null)
        {
            VMData.Accusation.HandList.RemoveObjectByDeck(card.Deck);
            if (finds.Name == gameContainer.DetectiveDetails!.Accusation.CharacterName)
            {
                gameContainer.DetectiveDetails.Accusation.CharacterName = "";
            }
            else if (finds.Name == gameContainer.DetectiveDetails!.Accusation.WeaponName)
            {
                gameContainer.DetectiveDetails.Accusation.WeaponName = "";
            }
            else if (finds.Name == gameContainer.DetectiveDetails!.Accusation.RoomName)
            {
                gameContainer.DetectiveDetails.Accusation.RoomName = "";
            }
            else
            {
                throw new CustomBasicException("Failed to find in the accusation details");
            }
            await privateAutoResume.SaveStateAsync(gameContainer);
            return true;
        }
        return false;
    }
    private async Task PredictionChangeMindAsync(ClueCardGameCardInformation card, int index)
    {
        bool rets = await RemovePredictionAsync(card);
        if (rets == false)
        {
            Toast.ShowUserErrorToast("I think there was a bug because did not find a card to remove for prediction");
        }
    }
    private async Task<bool> RemovePredictionAsync(ClueCardGameCardInformation card)
    {
        if (card.Name == mainGame.SaveRoot.CurrentPrediction!.FirstName)
        {
            mainGame.SaveRoot.CurrentPrediction.FirstName = "";
            gameContainer.DetectiveDetails!.CurrentPrediction!.FirstName = "";
            VMData.FirstName = "";
            VMData.Prediction.HandList.RemoveObjectByDeck(card.Deck);
            await privateAutoResume.SaveStateAsync(gameContainer);
            return true;
        }
        if (card.Name == mainGame.SaveRoot.CurrentPrediction.SecondName)
        {
            mainGame.SaveRoot.CurrentPrediction.SecondName = "";
            gameContainer.DetectiveDetails!.CurrentPrediction!.SecondName = "";
            VMData.SecondName = "";
            VMData.Prediction.HandList.RemoveObjectByDeck(card.Deck);
            await privateAutoResume.SaveStateAsync(gameContainer);
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
    public override bool CanEndTurn() => mainGame.SaveRoot.GameStatus == EnumClueStatusList.EndTurn && gameContainer.DetectiveDetails!.StartAccusation == false;
    public bool CanStartAccusation()
    {
        var player = mainGame.PlayerList.GetSelf();
        if (player.InGame == false)
        {
            return false;
        }
        if (mainGame.SaveRoot.GameStatus == EnumClueStatusList.FindClues)
        {
            return false;
        }
        return gameContainer.DetectiveDetails!.StartAccusation == false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task StartAccusationAsync()
    {
        gameContainer.DetectiveDetails!.StartAccusation = true;
        gameContainer.DetectiveDetails.Accusation = new();
        VMData.Accusation.ClearHand();
        VMData.Prediction.ClearHand(); //i think will clear hand here too.
        VMData.Accusation.Visible = true;
        VMData.Prediction.Visible = false;
        mainGame.SaveRoot.CurrentPrediction!.FirstName = "";
        mainGame.SaveRoot.CurrentPrediction.SecondName = "";
        mainGame.SaveRoot.LoadMod(VMData);
        gameContainer.DetectiveDetails.CurrentPrediction!.FirstName = "";
        gameContainer.DetectiveDetails.CurrentPrediction.SecondName = "";
        await privateAutoResume.SaveStateAsync(gameContainer);
        gameContainer.Command.UpdateSpecificAction(ClueActions.ToggleAccusation);
    }
    public bool CanCancelAccusation => gameContainer.DetectiveDetails!.StartAccusation;
    [Command(EnumCommandCategory.Game)]
    public async Task CancelAccusationAsync()
    {
        gameContainer.DetectiveDetails!.StartAccusation = false;
        gameContainer.DetectiveDetails.Accusation = new();
        VMData.Accusation.Visible = false;
        VMData.Prediction.Visible = true;
        VMData.Accusation.ClearHand(); //has to clear hand because you cancelled.  if you do again, redo.
        await privateAutoResume.SaveStateAsync(gameContainer);
        gameContainer.Command.UpdateSpecificAction(ClueActions.ToggleAccusation);
    }

    public bool CanAdd => mainGame.SaveRoot.GameStatus != EnumClueStatusList.FindClues;
    [Command(EnumCommandCategory.Game)]
    public async Task AddAsync(ClueCardGameCardInformation card)
    {
        if (gameContainer.DetectiveDetails!.StartAccusation)
        {
            await AddAccusationAsync(card);
            return;
        }
        if (CanAddPrediction)
        {
            await AddPredictionAsync(card);
            return;
        }
        //do nothing otherwise.
    }

    //public bool CanAddAccusation => gameContainer.DetectiveDetails!.StartAccusation && mainGame.SaveRoot.GameStatus != EnumClueStatusList.FindClues;
    //[Command(EnumCommandCategory.Game)]
    private async Task AddAccusationAsync(ClueCardGameCardInformation card)
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
            gameContainer.DetectiveDetails!.Accusation.WeaponName = card.Name;
        }
        else if (card.WhatType == EnumCardType.IsRoom)
        {
            gameContainer.DetectiveDetails!.Accusation.RoomName = card.Name;
        }
        else if (card.WhatType == EnumCardType.IsCharacter)
        {
            gameContainer.DetectiveDetails!.Accusation.CharacterName = card.Name;
        }
        else
        {
            throw new CustomBasicException("Failed to add accusation");
        }
        await privateAutoResume.SaveStateAsync(gameContainer);
    }
    private bool CanAddPrediction => mainGame.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction && gameContainer.DetectiveDetails!.StartAccusation == false;
    //[Command(EnumCommandCategory.Game)]
    private async Task AddPredictionAsync(ClueCardGameCardInformation card)
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
            mainGame.SaveRoot.CurrentPrediction!.FirstName = card.Name;
            gameContainer.DetectiveDetails!.CurrentPrediction!.FirstName = card.Name;
            VMData.FirstName = card.Name;
            newItem = new();
            newItem.Populate(card.Deck);
            VMData.Prediction.HandList.Add(newItem);
            await privateAutoResume.SaveStateAsync(gameContainer);
            return;
        }
        if (VMData.SecondName == "")
        {
            mainGame.SaveRoot.CurrentPrediction!.SecondName = card.Name;
            gameContainer.DetectiveDetails!.CurrentPrediction!.SecondName = card.Name;
            VMData.SecondName = card.Name;
            newItem = new();
            newItem.Populate(card.Deck);
            VMData.Prediction.HandList.Add(newItem);
            await privateAutoResume.SaveStateAsync(gameContainer);
            return;
        }
        Toast.ShowUserErrorToast("You already chose the 2 cards.  Click on a card in the prediction area to remove it");
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
            if (mainGame.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction)
            {
                return true;
            }

            return false;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakePredictionAsync()
    {
        mainGame!.SaveRoot!.CurrentPrediction!.FirstName = VMData.FirstName;
        mainGame.SaveRoot.CurrentPrediction.SecondName = VMData.SecondName;
        await mainGame.MakePredictionAsync();
    }
    public bool CanMakeAccusation()
    {
        if (mainGame.SaveRoot.GameStatus == EnumClueStatusList.FindClues)
        {
            return false;
        }
        if (gameContainer.DetectiveDetails!.Accusation.CharacterName == "")
        {
            return false;
        }
        if (gameContainer.DetectiveDetails!.Accusation.WeaponName == "")
        {
            return false;
        }
        if (gameContainer.DetectiveDetails!.Accusation.RoomName == "")
        {
            return false;
        }
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeAccusationAsync()
    {
        //this means you can make it because all is filled out.
        await mainGame.MakeAccusationAsync(gameContainer.DetectiveDetails!.Accusation);
    }
    protected override async Task ProcessHandClickedAsync(ClueCardGameCardInformation card, int index)
    {
        if (gameContainer.CanGiveCard(card) == false)
        {
            Toast.ShowUserErrorToast("Unable to give card as clue");
            return;
        }
        card.IsSelected = true;
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
                Deck = card.Deck,
                NickName = player.NickName
            };
            await gameContainer.Network!.SendAllAsync("cluegiven", hint);
            //only whoever turn it is needs to show who gave it.
            //has to send to everybody even though there is private autoresume.  that way can be recorded for everybody (for additional clues)
        }
        CommandContainer!.ManuelFinish = true;
        card.IsSelected = false;
        await mainGame.MarkCardAsync(tempPlayer, card, false);
        gameContainer.SaveRoot!.GameStatus = EnumClueStatusList.EndTurn;
        mainGame.OtherTurn = 0; //has to be 0 now for otherturn.
        if (gameContainer.BasicData.MultiPlayer == false)
        {
            throw new CustomBasicException("Computer should have never had this");
        }
        gameContainer.Network!.IsEnabled = true; //to wait for them to end turn.
    }
}