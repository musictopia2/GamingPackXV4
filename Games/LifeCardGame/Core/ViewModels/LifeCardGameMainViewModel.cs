namespace LifeCardGame.Core.ViewModels;
[InstanceGame]
public partial class LifeCardGameMainViewModel : BasicCardGamesVM<LifeCardGameCardInformation>
{
    private readonly LifeCardGameMainGameClass _mainGame;
    private readonly LifeCardGameVMData _model;
    private readonly IGamePackageResolver _resolver;
    private readonly LifeCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    public LifeCardGameMainViewModel(CommandContainer commandContainer,
        LifeCardGameMainGameClass mainGame,
        LifeCardGameVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        LifeCardGameGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _resolver = resolver;
        _gameContainer = gameContainer;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        _model.PlayerHand1.Maximum = 5;
        _model.CurrentPile.SendEnableProcesses(this, () => false);
        CommandContainer!.ExecutingChanged = CommandContainer_ExecutingChanged;
        _gameContainer.LoadOtherScreenAsync = LoadOtherScreenAsync;
        _gameContainer.CloseOtherScreenAsync = CloseOtherScreenAsync;
        CreateCommands();
        CreateCommands(commandContainer);
    }
    partial void CreateCommands();
    partial void CreateCommands(CommandContainer command);
    public async Task LoadOtherScreenAsync()
    {
        if (OtherScreen != null)
        {
            return;
        }
        OtherScreen = _resolver.Resolve<OtherViewModel>();
        await LoadScreenAsync(OtherScreen);
    }
    public async Task CloseOtherScreenAsync()
    {
        if (OtherScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(OtherScreen);
        OtherScreen = null;
    }
    public OtherViewModel? OtherScreen { get; set; }
    private void CommandContainer_ExecutingChanged()
    {
        _mainGame!.PlayerList!.EnableLifeStories(_mainGame, _model, !CommandContainer!.IsExecuting); //i think
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return _model.CurrentPile!.PileEmpty();
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int newDeck = _model.PlayerHand1!.ObjectSelected();
        if (newDeck == 0)
        {
            _toast.ShowUserErrorToast("Sorry, must select a card first");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("discard", newDeck);
        }
        await _mainGame!.DiscardAsync(newDeck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    [Command(EnumCommandCategory.Old)]
    public void YearsPassed()
    {
        _toast.ShowInfoToast($"{_mainGame.SaveRoot!.YearsPassed()} passed.  Once it reaches 60; the game is over");
    }
    public bool CanPlayCard => _model.CurrentPile.PileEmpty();
    [Command(EnumCommandCategory.Game)]
    public async Task PlayCardAsync()
    {
        int decks = _model.PlayerHand1.ObjectSelected();
        if (decks == 0)
        {
            _toast.ShowUserErrorToast("Must choose a card to play");
            return;
        }
        var thisCard = _mainGame.SingleInfo!.MainHandList.GetSpecificItem(decks);
        if (_mainGame.CanPlayCard(thisCard) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("playcard", decks);
        }
        CommandContainer!.ManuelFinish = true;
        await _mainGame.PlayCardAsync(thisCard);
    }
}