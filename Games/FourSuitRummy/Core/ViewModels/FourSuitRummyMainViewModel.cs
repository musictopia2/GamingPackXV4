namespace FourSuitRummy.Core.ViewModels;
[InstanceGame]
public partial class FourSuitRummyMainViewModel : BasicCardGamesVM<RegularRummyCard>
{
    private readonly FourSuitRummyMainGameClass _mainGame;
    private readonly FourSuitRummyVMData _model;
    private readonly IGamePackageResolver _resolver;
    private readonly IToast _toast;
    private readonly FourSuitRummyGameContainer _gameContainer;
    public FourSuitRummyMainViewModel(CommandContainer commandContainer,
        FourSuitRummyMainGameClass mainGame,
        FourSuitRummyVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        FourSuitRummyGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _resolver = resolver;
        _toast = toast;
        _gameContainer = gameContainer;
        _model.Deck1.NeverAutoDisable = true;
        var player = _mainGame.PlayerList.GetSelf();
        player.DoInit();
        _model.TempSets.Init(this);
        _model.TempSets.ClearBoard();
        _model.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        YourSetsScreen = _resolver.Resolve<PlayerSetsViewModel>();
        await LoadScreenAsync(YourSetsScreen);
        OpponentSetsScreen = _resolver.Resolve<PlayerSetsViewModel>();
        await LoadScreenAsync(OpponentSetsScreen);
    }
    public PlayerSetsViewModel? YourSetsScreen { get; set; }
    public PlayerSetsViewModel? OpponentSetsScreen { get; set; }
    protected override async Task TryCloseAsync()
    {
        await CloseSpecificChildAsync(YourSetsScreen!);
        await CloseSpecificChildAsync(OpponentSetsScreen!);
        await base.TryCloseAsync();
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
    public bool CanPlaySets => _gameContainer.AlreadyDrew;

    [Command(EnumCommandCategory.Game)]
    public async Task PlaySetsAsync()
    {
        BasicList<string> textList = new();
        var thisCol = _mainGame!.SetList();
        _mainGame.SingleInfo = _mainGame.PlayerList!.GetWhoPlayer();
        if (thisCol.Count == 0)
        {
            return;
        }
        if (_mainGame.Test!.DoubleCheck == true && thisCol.Count > 1)
        {
            throw new CustomBasicException("cannot have more than one for now for sets in beginning");
        }
        await thisCol.ForEachAsync(async thisInt =>
        {
            var temps = _model.TempSets.ObjectList(thisInt);
            var newCol = temps.ToRegularDeckDict();
            if (_mainGame.SingleInfo.MainSets!.CanAddSet(temps))
            {
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    var tempList = newCol.GetDeckListFromObjectList();
                    var thisStr = await js1.SerializeObjectAsync(tempList);
                    textList.Add(thisStr);
                }
                _model.TempSets.ClearBoard(thisInt);
                _mainGame.AddSet(newCol);
            }
        });
        if (_gameContainer.Test.DoubleCheck == true && textList.Count > 1)
        {
            throw new CustomBasicException("cannot have more than one for now for sets for sending to players");
        }
        if (_gameContainer.BasicData!.MultiPlayer == true && textList.Count > 0)
        {
            await _gameContainer.Network!.SendSeveralSetsAsync(textList, "finishedsets");
        }
        await _mainGame.ContinueTurnAsync();
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
}