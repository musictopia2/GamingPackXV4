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
        VMData.PlayerHand1.SendEnableProcesses(this, () => _mainGame.SaveRoot.GameStatus == EnumClueStatusList.FindClues);
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private async Task PredictionChangeMindAsync(ClueCardGameCardInformation card, int index)
    {
        bool rets = await RemovePredictionAsync(card);
        if (rets == false)
        {
            _toast.ShowUserErrorToast("I think there was a bug because did not find a card to remove");
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
    public override bool CanEndTurn() => _mainGame.SaveRoot.GameStatus == EnumClueStatusList.EndTurn;
    
    public bool CanAddPrediction => _mainGame.SaveRoot.GameStatus == EnumClueStatusList.MakePrediction;
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