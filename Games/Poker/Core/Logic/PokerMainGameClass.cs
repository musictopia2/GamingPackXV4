namespace Poker.Core.Logic;
[SingletonGame]
public class PokerMainGameClass : RegularDeckOfCardsGameClass<PokerCardInfo>, IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly ISystemError _error;
    private readonly IToast _toast;
    internal PokerSaveInfo _saveRoot;
    private struct PokerData
    {
        public string Message { get; set; }
        public decimal Mults { get; set; }
    }
    private readonly DeckRegularDict<PokerCardInfo> _cardList = new();
    internal bool GameGoing { get; set; }
    public PokerMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IGamePackageResolver container,
        ISystemError error,
        IToast toast
        )
    {
        _thisState = thisState;
        Aggregator = aggregator;
        _error = error;
        _toast = toast;
        _saveRoot = container.ReplaceObject<PokerSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
    }
    private decimal HowMuch
    {
        get
        {
            return _model!.BetAmount;
        }
    }
    private PokerMainViewModel? _model;
    public Task NewGameAsync(PokerMainViewModel model)
    {
        _model = model;
        GameGoing = true;
        _model.Round = 0;
        _model.Money = 200;
        return base.NewGameAsync(_model.DeckPile);
    }
    public override Task NewGameAsync(DeckObservablePile<PokerCardInfo> deck)
    {
        throw new CustomBasicException("Wrong");
    }
    internal async Task NewRoundAsync()
    {
        _model!.IsRoundOver = false;
        _model.Winnings = 0;
        _model.HandLabel = "Nothing";
        PokerMainViewModel.PokerList.Clear();
        _model.BetPlaced = false;
        _model.Round++;
        _cardList.Clear();
        await base.NewGameAsync(_model.DeckPile);
    }
    public IEventAggregator Aggregator { get; }
    public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
    {
        return await _thisState.CanOpenSavedSinglePlayerGameAsync();
    }
    public override async Task OpenSavedGameAsync()
    {
        DeckList.OrderedObjects(); //i think
        _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<PokerSaveInfo>();
        if (_saveRoot.DeckList.Count > 0)
        {
            var newList = _saveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
            DeckPile!.OriginalList(newList);
        }
    }
    private bool _isBusy;
    public async Task SaveStateAsync()
    {
        if (_isBusy)
        {
            return;
        }
        _isBusy = true;
        _saveRoot.DeckList = DeckPile!.GetCardIntegers();
        await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot); //i think
        _isBusy = false;
    }
    public async Task ShowWinAsync()
    {
        _toast.ShowSuccessToast("Congratulations, you won");
        GameGoing = false;
        await this.SendGameOverAsync(_error);
    }
    protected override void AfterShuffle() { }
    internal void DrawFromDeck()
    {
        int maxs = PokerMainViewModel.SpotsToFill;
        bool canEndRound = _model!.BetPlaced;
        _model.BetPlaced = true;
        var thisList = _model.DeckPile!.DrawSeveralCards(maxs);
        PokerMainViewModel.PopulateNewCards(thisList);
        ProcessScores();
        _model.IsRoundOver = canEndRound;
        if (canEndRound == false)
        {
            return;
        }
        _model.Money += _model.Winnings;
    }
    private void ProcessScores()
    {
        var tempList = PokerMainViewModel.GetCardList;
        _cardList.ReplaceRange(tempList);
        var poks = GetPokerInfo();
        _model!.HandLabel = poks.Message;
        _model.Winnings = poks.Mults * HowMuch;
    }
    private PokerData GetPokerInfo()
    {
        PokerData output = new();
        ScoreInfo scores = new();
        scores.CardList = _cardList.ToRegularDeckDict();
        if (scores.IsRoyalFlush())
        {
            output.Message = "Royal flush";
            output.Mults = 128;
            return output;
        }
        if (scores.IsStraightFlush())
        {
            output.Message = "Straight flush";
            output.Mults = 64;
            return output;
        }
        if (scores.Kinds(4))
        {
            output.Message = "Four of a kind";
            output.Mults = 32;
            return output;
        }
        if (scores.IsFullHouse())
        {
            output.Message = "Full house";
            output.Mults = 16;
            return output;
        }
        if (scores.IsFlush())
        {
            output.Message = "Flush";
            output.Mults = 8;
            return output;
        }
        if (scores.IsStraight())
        {
            output.Message = "Straight";
            output.Mults = 4;
            return output;
        }
        if (scores.Kinds(3))
        {
            output.Message = "Three of a kind";
            output.Mults = 2;
            return output;
        }
        if (scores.MultiPair())
        {
            output.Message = "Two pairs";
            output.Mults = 1;
            return output;
        }
        if (scores.Kinds(2))
        {
            output.Message = "Pair";
            output.Mults = 0;
            return output;
        }
        if (scores.HasAce())
        {
            output.Message = "High card ace";
            output.Mults = -0.5m;
            return output;
        }
        output.Message = "Nothing";
        output.Mults = -1;
        return output;
    }
}