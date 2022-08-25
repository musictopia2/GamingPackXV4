namespace CribbagePatience.Core.ViewModels;
[InstanceGame]
public partial class CribbagePatienceMainViewModel : ScreenViewModel,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer,
        IHandle<HandScoresEventModel>

{
    private readonly BasicData _basicData;
    private readonly IToast _toast;
    private readonly ISystemError _error;
    private readonly IMessageBox _message;
    private readonly CribbagePatienceMainGameClass _mainGame;
    public SingleObservablePile<CribbageCard>? StartPile;
    public HandObservable<CribbageCard> TempCrib;
    public HandObservable<CribbageCard> Hand1;
    public BasicList<ScoreHandCP>? HandScores;
    public ScoreSummaryCP? Scores;
    public ScoreHandCP GetScoreHand(EnumHandCategory thisCategory)
    {
        return HandScores!.Single(items => items.HandCategory == thisCategory);
    }
    public (int row, int column) GetRowColumn(EnumHandCategory thisCategory)
    {
        var hand = GetScoreHand(thisCategory);
        return hand.GetRowColumn();
    }
    public DeckObservablePile<CribbageCard> DeckPile { get; set; }
    public bool CanCrib => Hand1.Visible;
    [Command(EnumCommandCategory.Plain)]
    public async Task CribAsync()
    {
        int manys = Hand1.HowManySelectedObjects;
        if (manys == 0)
        {
            _toast.ShowUserErrorToast("Must choose cards");
            return;
        }
        if (manys != 2)
        {
            _toast.ShowUserErrorToast("Must choose 2 cards for crib");
            return;
        }
        var thisList = Hand1.ListSelectedObjects(true);
        if (Hand1.HandList.Count == 6)
        {
            throw new CustomBasicException("Did not remove cards before starting to put to crib");
        }
        _mainGame.RemoveTempCards(thisList);
        _mainGame.CardsToCrib(thisList);
        if (DeckPile.IsEndOfDeck())
        {
            CommandContainer.UpdateAll(); //needs to show the updates.
            await _message.ShowMessageAsync("Game Over.  Check Results");
            await _mainGame.SendGameOverAsync(_error);
        }
    }
    public bool CanContinue()
    {
        if (Hand1.Visible == true)
        {
            return false;
        }
        return !DeckPile.IsEndOfDeck();
    }
    [Command(EnumCommandCategory.Plain)]
    public void Continue()
    {
        _mainGame.NewRound();
    }
    public CribbagePatienceMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        BasicData basicData,
        IToast toast,
        ISystemError error,
        IMessageBox message
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        _toast = toast;
        _error = error;
        _message = message;
        CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
        DeckPile = resolver.ReplaceObject<DeckObservablePile<CribbageCard>>();
        DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = false;
        DeckPile.SendEnableProcesses(this, () =>
        {
            return false;
        });

        Hand1 = new(commandContainer);
        Hand1.Visible = false; //has to be proven true.
        Hand1.Maximum = 6;
        Hand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        _mainGame = resolver.ReplaceObject<CribbagePatienceMainGameClass>(); //hopefully this works.  means you have to really rethink.
        _mainGame._saveRoot.HandScores = new();
        3.Times(x =>
        {
            ScoreHandCP tempHand = new();
            tempHand.HandCategory = (EnumHandCategory)x;
            _mainGame._saveRoot.HandScores.Add(tempHand);
        });
        StartPile = new(CommandContainer);
        StartPile.Text = "Start Card";
        StartPile.CurrentOnly = true;
        StartPile.SendEnableProcesses(this, () => false);
        Scores = new();
        TempCrib = new(CommandContainer);
        TempCrib.Visible = false;
        TempCrib.Text = "Crib So Far";
        TempCrib.Maximum = 4;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    private async Task DeckPile_DeckClickedAsync()
    {
        await Task.CompletedTask;
    }
    private async void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer.IsExecuting)
        {
            return;
        }
        if (_mainGame.GameGoing)
        {
            await _mainGame.SaveStateAsync();
        }
    }
    public CommandContainer CommandContainer { get; set; }
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = false;
        await base.ActivateAsync();
        await _mainGame.NewGameAsync(this);
        Hand1.IsEnabled = true; //somehow a timing issue.  this should fix this one.  not sure if its going to be more of a problem later or not (?)
        CommandContainer.UpdateAll();
    }
    protected override Task TryCloseAsync()
    {
        CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
        return base.TryCloseAsync();
    }
    void IHandle<HandScoresEventModel>.Handle(HandScoresEventModel message)
    {
        HandScores = message.HandScores;
    }
}