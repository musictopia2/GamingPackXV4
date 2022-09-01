namespace Fluxx.Core.ViewModels;
[InstanceGame]
public partial class FluxxMainViewModel : BasicCardGamesVM<FluxxCardInformation>, IShowKeeperVM
{
    private readonly FluxxMainGameClass _mainGame;
    private readonly FluxxVMData _model;
    private readonly IGamePackageResolver _resolver;
    private readonly FluxxGameContainer _gameContainer;
    private readonly IDiscardProcesses _discardProcesses;
    private readonly IAnalyzeProcesses _analyzeQueProcesses;
    private readonly KeeperContainer _keeperContainer;
    private readonly FluxxDelegates _delegates;
    private readonly IToast _toast;
    public FluxxMainViewModel(CommandContainer commandContainer,
        FluxxMainGameClass mainGame,
        FluxxVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        FluxxGameContainer gameContainer,
        IDiscardProcesses discardProcesses,
        IAnalyzeProcesses analyzeQueProcesses,
        KeeperContainer keeperContainer,
        FluxxDelegates delegates,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _resolver = resolver;
        _gameContainer = gameContainer;
        _discardProcesses = discardProcesses;
        _analyzeQueProcesses = analyzeQueProcesses;
        _keeperContainer = keeperContainer;
        _delegates = delegates;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        _gameContainer.LoadGiveAsync = LoadGiveAsync;
        _gameContainer.LoadPlayAsync = LoadPlayAsync;
        _model.Keeper1.ConsiderSelectOneAsync += OnConsiderSelectOneCardAsync;
        _model.Goal1.ConsiderSelectOneAsync += OnConsiderSelectOneCardAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override async Task TryCloseAsync()
    {
        await ClosePlayGiveAsync();
        _model.Keeper1.ConsiderSelectOneAsync -= OnConsiderSelectOneCardAsync;
        _model.Goal1.ConsiderSelectOneAsync -= OnConsiderSelectOneCardAsync;
        await ClosePlayGiveAsync(); //just in case.
        await base.TryCloseAsync();
    }
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        if (_gameContainer!.CurrentAction != null && _gameContainer.CurrentAction.Deck == EnumActionMain.Taxation)
        {
            await LoadGiveAsync();
        }
        else
        {
            await LoadPlayAsync();
        }
    }
    private async Task ClosePlayGiveAsync()
    {
        if (PlayGiveScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(PlayGiveScreen);
        PlayGiveScreen = null;
    }
    private async Task LoadPlayAsync()
    {
        await ClosePlayGiveAsync();
        PlayGiveScreen = _resolver.Resolve<PlayViewModel>();
        await LoadScreenAsync(PlayGiveScreen);
    }
    private async Task LoadGiveAsync()
    {
        await ClosePlayGiveAsync();
        PlayGiveScreen = _resolver.Resolve<GiveViewModel>();
        await LoadScreenAsync(PlayGiveScreen);
    }
    public IScreen? PlayGiveScreen { get; set; }
    ICustomCommand IShowKeeperVM.ShowKeepersCommand => ShowKeepersCommand!;

    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        return _mainGame.OtherTurn == 0;
    }
    public override async Task EndTurnAsync()
    {
        var ends = _mainGame!.StatusEndRegularTurn();
        if (ends == EnumEndTurnStatus.Goal)
        {
            _toast.ShowUserErrorToast("Sorry; you must get rid of excess goals");
            return;
        }
        if (ends == EnumEndTurnStatus.Play)
        {
            _toast.ShowUserErrorToast("Sorry; you have plays remaining");
            return;
        }
        if (ends == EnumEndTurnStatus.Hand)
        {
            _toast.ShowUserErrorToast("Sorry; you have too many cards in your hand");
            return;
        }
        if (ends == EnumEndTurnStatus.Keeper)
        {
            _toast.ShowUserErrorToast("Sorry; you have too many keepers");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendEndTurnAsync();
        }
        await _mainGame.EndTurnAsync();
    }
    protected override Task OnConsiderSelectOneCardAsync(FluxxCardInformation thisObject)
    {
        if (thisObject.Deck == _model.CardDetail!.CurrentCard.Deck)
        {
            _model.CardDetail.ResetCard();
        }
        else
        {
            _model.CardDetail.ShowCard(thisObject);
        }
        return Task.CompletedTask;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task DiscardAsync()
    {
        int goalDiscarded = _model.Goal1!.ObjectSelected();
        int keepers = _model.Keeper1!.HowManySelectedObjects;
        int yours = _model.PlayerHand1!.HowManySelectedObjects;
        if (goalDiscarded == 0 && keepers == 0 && yours == 0)
        {
            _toast.ShowUserErrorToast("There is no cards selected to discard");
            return;
        }
        BasicList<int> tempList = new() { keepers, goalDiscarded, yours };
        if (tempList.Count(item => item > 0) > 1)
        {
            _toast.ShowUserErrorToast("Can choose a goal, keepers, or from your hand; not from more than one");
            return;
        }
        if (goalDiscarded > 0)
        {
            if (_gameContainer!.NeedsToRemoveGoal() == false)
            {
                _toast.ShowUserErrorToast("Cannot discard any goals");
                _model.UnselectAllCards();
                return;
            }
            if (goalDiscarded == (int)_mainGame.SaveRoot!.GoalList.Last().Deck && _mainGame.SaveRoot.GoalList.Count == 3)
            {
                _toast.ShowUserErrorToast("Cannot discard the third goal on the list.  Must choose one of the 2 that was there before");
                _model.UnselectAllCards();
                return;
            }
            await _discardProcesses.DiscardGoalAsync(goalDiscarded);
            await _analyzeQueProcesses.AnalyzeQueAsync();
            return;
        }
        if (_gameContainer!.NeedsToRemoveGoal())
        {
            _toast.ShowUserErrorToast("Must discard a goal before discarding anything else");
            _model.UnselectAllCards();
            return;
        }
        if (yours > 0)
        {
            if (_mainGame.SaveRoot!.HandLimit == -1)
            {
                _toast.ShowUserErrorToast("There is no hand limit.  Therefore cannot discard any cards from your hand");
                _model.UnselectAllCards();
                return;
            }
            int newCount = _mainGame.SingleInfo!.MainHandList.Count - yours;
            if (newCount < _mainGame.SaveRoot.HandLimit)
            {
                _toast.ShowUserErrorToast($"Cannot discard from hand {yours} cards because that will cause you to discard too many cards");
                _model.UnselectAllCards();
                return;
            }
            var firstList = _model.PlayerHand1.ListSelectedObjects();
            await _discardProcesses.DiscardFromHandAsync(firstList);
            return;
        }
        if (keepers > 0)
        {
            if (_mainGame.SaveRoot!.KeeperLimit == -1)
            {
                _toast.ShowUserErrorToast("There is no keeper limit.  Therefore; cannot discard any keepers");
                _model.UnselectAllCards();
                return;
            }
            int newCount = _mainGame.SingleInfo!.KeeperList.Count - keepers;
            if (newCount < _mainGame.SaveRoot.KeeperLimit)
            {
                _toast.ShowUserErrorToast($"Cannot discard from keepers {keepers} cards because that will cause you to discard too many keepers");
                _model.UnselectAllCards();
                return;
            }
            var firstList = _model.Keeper1.ListSelectedObjects();
            DeckRegularDict<FluxxCardInformation> finList = new(firstList);
            await _discardProcesses.DiscardKeepersAsync(finList);
            return;
        }
        throw new CustomBasicException("Don't know how to discard from here");
    }
    [Command(EnumCommandCategory.Game)]
    public void SelectHandCards()
    {
        _model.PlayerHand1.SelectAllObjects();
    }
    [Command(EnumCommandCategory.Game)]
    public void UnselectHandCards()
    {
        _model.PlayerHand1.UnselectAllObjects();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ShowKeepersAsync()
    {
        _keeperContainer.ShowKeepers();
        if (_delegates.LoadKeeperScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the keeper screen when main needs to load it");
        }
        await _delegates.LoadKeeperScreenAsync.Invoke(_keeperContainer);
    }
}