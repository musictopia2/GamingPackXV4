namespace CribbagePatience.Core.ViewModels;
[InstanceGame]
public partial class CribbagePatienceMainViewModel(CommandContainer commandContainer,
    IEventAggregator aggregator,
    IGamePackageResolver resolver,
        BasicData basicData,
        IToast toast,
        ISystemError error,
        IMessageBox message
        ) : IBasicEnableProcess,
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>,
        IHandle<HandScoresEventModel>,
    IStateChanged

{
    private partial void Subscribe();
    private partial void Unsubscribe();
    private CribbagePatienceMainGameClass _mainGame = null!;
    public SingleObservablePile<CribbageCard>? StartPile;
    public HandObservable<CribbageCard> TempCrib = null!;
    public HandObservable<CribbageCard> Hand1 = null!;
    public BasicList<ScoreHandCP>? HandScores;
    public ScoreSummaryCP? Scores;

    public event Action? StateChanged;

    public ScoreHandCP GetScoreHand(EnumHandCategory thisCategory)
    {
        return HandScores!.Single(items => items.HandCategory == thisCategory);
    }
    public (int row, int column) GetRowColumn(EnumHandCategory thisCategory)
    {
        var hand = GetScoreHand(thisCategory);
        return hand.GetRowColumn();
    }
    public DeckObservablePile<CribbageCard> DeckPile { get; set; } = null!;
    public bool CanCrib => Hand1.Visible;
    [Command(EnumCommandCategory.Plain)]
    public async Task CribAsync()
    {
        int manys = Hand1.HowManySelectedObjects;
        if (manys == 0)
        {
            toast.ShowUserErrorToast("Must choose cards");
            return;
        }
        if (manys != 2)
        {
            toast.ShowUserErrorToast("Must choose 2 cards for crib");
            return;
        }
        var thisList = Hand1.ListSelectedObjects(true);
        if (Hand1.HandList.Count == 6)
        {
            throw new CustomBasicException("Did not remove cards before starting to put to crib");
        }
        _mainGame.RemoveTempCards(thisList);
        _mainGame.CardsToCrib(thisList);
        StateChanged?.Invoke();
        if (DeckPile.IsEndOfDeck())
        {
            await message.ShowMessageAsync("Game Over.  Check Results");
            await _mainGame.SendGameOverAsync(error);
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
        StateChanged?.Invoke();
    }
    void IMainGameVM.Initialize()
    {
        Subscribe();
        commandContainer.ExecutingChanged = CommandContainer_ExecutingChanged; //hopefully no problem (?)
        DeckPile = resolver.ReplaceObject<DeckObservablePile<CribbageCard>>();
        DeckPile.DeckClickedAsync = DeckPile_DeckClickedAsync;
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
        _mainGame._saveRoot.HandScores = [];
        3.Times(x =>
        {
            ScoreHandCP tempHand = new();
            tempHand.HandCategory = (EnumHandCategory)x;
            _mainGame._saveRoot.HandScores.Add(tempHand);
        });
        StartPile = new(commandContainer);
        StartPile.Text = "Start Card";
        StartPile.CurrentOnly = true;
        StartPile.SendEnableProcesses(this, () => false);
        Scores = new();
        TempCrib = new(commandContainer);
        TempCrib.Visible = false;
        TempCrib.Text = "Crib So Far";
        TempCrib.Maximum = 4;
        CreateCommands(commandContainer);
    }

    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        basicData.GameDataLoading = false;
        await _mainGame.NewGameAsync(this);
    }

    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
        commandContainer.ExecutingChanged = null;
    }
    partial void CreateCommands(CommandContainer container);
    private async Task DeckPile_DeckClickedAsync()
    {
        await Task.CompletedTask;
    }
    private async void CommandContainer_ExecutingChanged()
    {
        if (commandContainer.IsExecuting)
        {
            return;
        }
        if (_mainGame.GameGoing)
        {
            await _mainGame.SaveStateAsync();
        }
    }
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    void IHandle<HandScoresEventModel>.Handle(HandScoresEventModel message)
    {
        HandScores = message.HandScores;
    }
}