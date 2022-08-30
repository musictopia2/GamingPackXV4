namespace FiveCrowns.Core.ViewModels;
[InstanceGame]
public partial class FiveCrownsMainViewModel : BasicCardGamesVM<FiveCrownsCardInformation>
{
    private readonly FiveCrownsMainGameClass _mainGame;
    private readonly FiveCrownsVMData _model;
    private readonly FiveCrownsGameContainer _gameContainer;
    private readonly IToast _toast;
    public FiveCrownsMainViewModel(CommandContainer commandContainer,
        FiveCrownsMainGameClass mainGame,
        FiveCrownsVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        FiveCrownsGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _gameContainer = gameContainer;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = false;
        var player = _mainGame.PlayerList.GetSelf();
        player.DoInit();
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        _model.TempSets.Init(this);
        _model.TempSets.ClearBoard();
        _model.TempSets.SetClickedAsync += TempSets_SetClickedAsync;
        _model.MainSets.SendEnableProcesses(this, () => false);
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.OutOfTurn)]
    public void Back() //you can put back even if its not your turn.
    {
        var thisList = _model.TempSets!.ListSelectedObjects();
        thisList.ForEach(thisCard =>
        {
            thisCard.IsSelected = false;
            _model.TempSets.RemoveObject(thisCard.Deck);
        });
        //no private save here now.
        FiveCrownsPlayerItem thisPlayer = _mainGame!.PlayerList!.GetSelf();
        thisPlayer.MainHandList.AddRange(thisList);
        _mainGame.SortCards();
    }
    public bool CanLayDownSets()
    {
        if (_gameContainer!.AlreadyDrew == false)
        {
            return false;
        }
        return !_mainGame.SaveRoot!.SetsCreated;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task LayDownSetsAsync()
    {
        if (_mainGame!.SaveRoot!.PlayerWentOut > 0)
        {
            bool lats = _mainGame.CanLaterLayDown();
            if (lats == false)
            {
                return;
            }
            var thisCol1 = _mainGame.ListValidSets();
            await ProcessValidSetsAsync(thisCol1);
            return;
        }
        bool rets = _mainGame.HasInitialSet();
        if (rets == false)
        {
            _toast.ShowUserErrorToast("Sorry, you do not have the valid sets needed to go out");
            return;
        }
        var thisCol2 = _mainGame.ListValidSets();
        await ProcessValidSetsAsync(thisCol2);
    }
    protected override Task TryCloseAsync()
    {
        _model.TempSets.SetClickedAsync -= TempSets_SetClickedAsync;
        return base.TryCloseAsync();
    }
    protected override bool CanEnableDeck()
    {
        return !_gameContainer!.AlreadyDrew;
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
    public override bool CanEnableAlways()
    {
        return true;
    }
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
    private async Task ProcessValidSetsAsync(BasicList<TempInfo> thisCol)
    {
        BasicList<string> newList = new();
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                var thisStr = await js.SerializeObjectAsync(tempList);
                newList.Add(thisStr);
            }
            _model.TempSets!.ClearBoard(thisTemp.SetNumber);
            _mainGame!.CreateSet(thisTemp.CardList);
        });
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendSeveralSetsAsync(newList, "finishedsets");
        }
        await _mainGame!.FinishedSetsAsync();
    }
}