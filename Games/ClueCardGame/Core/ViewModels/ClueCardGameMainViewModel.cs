namespace ClueCardGame.Core.ViewModels;
[InstanceGame]
public partial class ClueCardGameMainViewModel : BasicCardGamesVM<ClueCardGameCardInformation>
{
    private readonly ClueCardGameMainGameClass _mainGame; //if we don't need, delete.
    private readonly IToast _toast;
    private readonly ClueCardGameGameContainer _gameContainer;
    public ClueCardGameVMData VMData { get; set; }
    public ClueCardGameMainViewModel(CommandContainer commandContainer,
        ClueCardGameMainGameClass mainGame,
        ClueCardGameVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast,
        ClueCardGameGameContainer gameContainer
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _toast = toast;
        _gameContainer = gameContainer;
        CreateCommands(commandContainer);
    }

    partial void CreateCommands(CommandContainer command);

    //anything else needed is here.
    //if i need something extra, will add to template as well.
    protected override bool CanEnableDeck()
    {
        //todo:  decide whether to enable deck.
        return false; //otherwise, can't compile.
    }
    protected override bool CanEnablePile1()
    {
        //todo:  decide whether to enable deck.
        return false; //otherwise, can't compile.
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        //if we have anything, will be here.
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public bool CanMakePrediction
    {
        get
        {
            if (VMData.FirstName  == "" || VMData.SecondName == "")
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
            //has to send to everybody.  that way the host can record (since recording must be done).
            //await _gameContainer.Network!.SendToParticularPlayerAsync("cluegiven", payLoad.Deck, tempPlayer.NickName);
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