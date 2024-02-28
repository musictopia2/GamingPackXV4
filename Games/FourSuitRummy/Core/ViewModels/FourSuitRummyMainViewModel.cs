namespace FourSuitRummy.Core.ViewModels;
[InstanceGame]
public partial class FourSuitRummyMainViewModel : BasicCardGamesVM<RegularRummyCard>
{
    private readonly FourSuitRummyMainGameClass _mainGame;
    private readonly FourSuitRummyVMData _model;
    private readonly IGamePackageResolver _resolver;
    private readonly IToast _toast;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    private readonly FourSuitRummyGameContainer _gameContainer;
    public FourSuitRummyMainViewModel(CommandContainer commandContainer,
        FourSuitRummyMainGameClass mainGame,
        FourSuitRummyVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        FourSuitRummyGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _resolver = resolver;
        _toast = toast;
        _privateAutoResume = privateAutoResume;
        _gameContainer = gameContainer;
        _model.Deck1.NeverAutoDisable = true;
        var player = _mainGame.PlayerList.GetSelf();
        player.DoInit();
        _model.TempSets.Init(this);
        _model.TempSets.ClearBoard();
        PossibleAutoResume();
        _model.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private void PossibleAutoResume()
    {
        if (_gameContainer.TempSets.Count > 0)
        {
            var player = _gameContainer.PlayerList!.GetSelf();
            bool hadAny = false;
            foreach (var item in _gameContainer.TempSets)
            {
                var current = _model.TempSets.SetList[item.SetNumber - 1];
                var cards = item.Cards.GetNewObjectListFromDeckList(_gameContainer.DeckList);
                DeckRegularDict<RegularRummyCard> toAdd = [];
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
                _model.TempSets.PublicCount();
            }
        }
    }
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
    private async Task TempSets_SetClickedAsync(int index)
    {
        if (_isProcessing == true)
        {
            return;
        }
        _isProcessing = true;
        var tempList = _model.PlayerHand1!.ListSelectedObjects(true);
        _model.TempSets!.AddCards(index, tempList);
        await _privateAutoResume.SaveStateAsync(_model);
        _isProcessing = false;
        return;
    }
    public bool CanPlaySets => _gameContainer.AlreadyDrew;

    [Command(EnumCommandCategory.Game)]
    public async Task PlaySetsAsync()
    {
        BasicList<string> textList = [];
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