namespace Millebournes.Core.ViewModels;
[InstanceGame]
public partial class MillebournesMainViewModel : BasicCardGamesVM<MillebournesCardInformation>
{
    private readonly MillebournesMainGameClass _mainGame;
    private readonly MillebournesVMData _model;
    private readonly IGamePackageResolver _resolver;
    private readonly MillebournesGameContainer _gameContainer;
    private readonly IToast _toast;
    public MillebournesMainViewModel(CommandContainer commandContainer,
        MillebournesMainGameClass mainGame,
        MillebournesVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        MillebournesGameContainer gameContainer,
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
        _model.Deck1.NeverAutoDisable = false;
        _model.Pile2!.PileClickedAsync = Pile2_PileClickedAsync;
        CommandContainer!.ExecutingChanged = CommandContainer_ExecutingChanged;
        _model.Pile1.Text = "New Card";
        _model.Pile1.Visible = false;
        _model.Pile2.Text = "Throw Away";
        _model.Pile2.FirstLoad(new MillebournesCardInformation());
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.None;
        _model.PlayerHand1.Maximum = 6;
        _gameContainer.TeamClickAsync = ProcessTeamClickAsync;
        _gameContainer.LoadCoupeAsync = LoadCoupeAsync;
        _gameContainer.CloseCoupeAsync = CloseCoupeAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CoupeViewModel? CoupeScreen { get; set; }
    private async Task LoadCoupeAsync()
    {
        if (CoupeScreen != null)
        {
            throw new CustomBasicException("Coupe screen was already loaded.  Rethink");
        }
        CoupeScreen = _resolver.Resolve<CoupeViewModel>();
        await LoadScreenAsync(CoupeScreen);
    }
    private async Task CloseCoupeAsync()
    {
        if (CoupeScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(CoupeScreen);
        CoupeScreen = null;
    }
    private void CommandContainer_ExecutingChanged()
    {
        _model.Pile1.ReportCanExecuteChange();
        _gameContainer!.TeamList.ForEach(thisTeam =>
        {
            thisTeam.EnableChange();
        });
    }
    private async Task Pile2_PileClickedAsync()
    {
        int deck = _mainGame!.FindDeck;
        if (deck == 0)
        {
            _toast.ShowUserErrorToast("Sorry, needs to select a card to throw away");
            return;
        }
        await _mainGame!.ThrowawayCardAsync(deck);
    }
    protected override async Task TryCloseAsync()
    {
        await CloseCoupeAsync();
        await base.TryCloseAsync();
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return _model.Pile1!.Visible;
    }
    protected override Task ProcessDiscardClickedAsync()
    {
        if (_model.Pile1!.PileEmpty())
        {
            throw new CustomBasicException("Since there is no current card, should have never been enabled");
        }
        _model.PlayerHand1!.UnselectAllObjects();
        if (_model.Pile1.CardSelected() == 0)
        {
            _model.Pile1.IsSelected(true);
        }
        else
        {
            _model.Pile1.IsSelected(false);
        }
        CommandContainer!.ManuelFinish = false;
        CommandContainer.IsExecuting = false;
        return Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    protected override async Task ProcessHandClickedAsync(MillebournesCardInformation thisCard, int index)
    {
        if (thisCard.IsSelected == true)
        {
            thisCard.IsSelected = false;
            return;
        }
        if (_model.Pile1!.CardSelected() > 0)
        {
            _model.Pile1.IsSelected(false);
            thisCard.IsSelected = true;
            return;
        }
        _model.PlayerHand1!.UnselectAllObjects();
        thisCard.IsSelected = true;
        await Task.CompletedTask;
    }
    private async Task ProcessTeamClickAsync(EnumPileType pileType, int team)
    {
        int newDeck = _mainGame!.FindDeck;
        if (pileType == EnumPileType.None)
        {
            throw new CustomBasicException("Must have a pile that was clicked on");
        }
        if (team == 0)
        {
            throw new CustomBasicException("No team sent when clicking a pile");
        }
        if (newDeck == 0)
        {
            _toast.ShowUserErrorToast("Sorry, you must select a card first");
            return;
        }
        var thisTeam = _gameContainer!.FindTeam(team);
        thisTeam.CurrentCard = _gameContainer.DeckList!.GetSpecificItem(newDeck);
        string message;
        if (pileType == EnumPileType.Miles)
        {
            if (team != _mainGame.SaveRoot!.CurrentTeam)
            {
                throw new CustomBasicException($"The miles should have been disabled since you cannot play miles for another team.  The team clicked was {team} .  However, the current team is {_mainGame.SaveRoot.CurrentTeam}");
            }
            if (thisTeam.CanPlaceMiles(out message) == false)
            {
                _toast.ShowUserErrorToast(message);
                _model.PlayerHand1!.UnselectAllObjects();
                return;
            }
        }
        if (pileType == EnumPileType.Hazard)
        {
            if (_mainGame.SaveRoot!.CurrentTeam == team)
            {
                if (thisTeam.CanFixHazard(out message) == false)
                {
                    _toast.ShowUserErrorToast(message);
                    _model.PlayerHand1!.UnselectAllObjects();
                    return;
                }
            }
            else if (_mainGame.SaveRoot.CurrentTeam != team && thisTeam.CanGiveHazard(out message) == false)
            {
                _toast.ShowUserErrorToast(message);
                _model.PlayerHand1!.UnselectAllObjects();
                return;
            }
        }
        if (pileType == EnumPileType.Speed)
        {
            if (_mainGame.SaveRoot!.CurrentTeam == team)
            {
                if (thisTeam.CanEndSpeedLimit(out message) == false)
                {
                    _toast.ShowUserErrorToast(message);
                    _model.PlayerHand1!.UnselectAllObjects();
                    return;
                }
            }
            else if (_mainGame.SaveRoot.CurrentTeam != team && thisTeam.CanGiveSpeedLimit(out message) == false)
            {
                _toast.ShowUserErrorToast(message);
                _model.PlayerHand1!.UnselectAllObjects();
                return;
            }
        }
        if (pileType == EnumPileType.Safety)
        {
            if (_mainGame.SaveRoot!.CurrentTeam != team)
            {
                throw new CustomBasicException("Cannot place a safety for another team.  Therefore, this should have been disabled");
            }
            if (thisTeam.CanPlaceSafety(out message) == false)
            {
                _toast.ShowUserErrorToast(message);
                _model.PlayerHand1!.UnselectAllObjects();
                return;
            }
        }
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            SendPlay thisSend = new();
            thisSend.Deck = newDeck;
            thisSend.Pile = pileType;
            thisSend.Team = team;
            await _mainGame.Network!.SendAllAsync("regularplay", thisSend);
        }
        await _mainGame.PlayAsync(newDeck, pileType, team, false);
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task SafetyClickAsync(TeamCP team)
    {
        if (_gameContainer.TeamClickAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the team click.  Rethink");
        }
        await _gameContainer.TeamClickAsync.Invoke(EnumPileType.Safety, team.TeamNumber);
    }
}