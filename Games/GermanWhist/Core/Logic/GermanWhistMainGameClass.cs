namespace GermanWhist.Core.Logic;
[SingletonGame]
public class GermanWhistMainGameClass
    : TrickGameClass<EnumSuitList, GermanWhistCardInformation, GermanWhistPlayerItem, GermanWhistSaveInfo>
        , ISerializable
{
    private readonly GermanWhistVMData _model;
    private readonly CommandContainer _command;
    private readonly IAdvancedTrickProcesses _aTrick;
    public GermanWhistMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        GermanWhistVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<GermanWhistCardInformation> cardInfo,
        CommandContainer command,
        GermanWhistGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        _aTrick = aTrick;
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true;
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.DoubleCheck == true)
        {
            return; //so will be stuck.  this way i can test the human player first.
        }
        if (Test.NoAnimations == true)
        {
            await Delay!.DelaySeconds(.75);
        }
        var MoveList = SingleInfo!.MainHandList.Where(card => IsValidMove(card.Deck)).Select(card => card.Deck).ToBasicList();
        await PlayCardAsync(MoveList.GetRandomItem());
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        LoadVM();
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TricksWon = 0;
        });
        SaveRoot!.WasEnd = false;
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        var thisCard = _model!.Deck1!.RevealCard();
        SaveRoot!.TrumpSuit = thisCard.Suit;
        _aTrick!.ClearBoard();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        this.ShowTurn();
        await SaveStateAsync();
        await ContinueTurnAsync();
    }
    private int WhoWonTrick(DeckRegularDict<GermanWhistCardInformation> thisCol)
    {
        GermanWhistCardInformation leadCard = thisCol.First();
        var thisCard = thisCol.Last();
        if (thisCard.Suit == SaveRoot!.TrumpSuit && leadCard.Suit != SaveRoot.TrumpSuit)
        {
            return WhoTurn;
        }
        if (leadCard.Suit == SaveRoot.TrumpSuit && thisCard.Suit != SaveRoot.TrumpSuit)
        {
            return leadCard.Player;
        }
        if (thisCard.Suit == leadCard.Suit)
        {
            if (thisCard.Value > leadCard.Value)
            {
                return WhoTurn;
            }
        }
        return leadCard.Player;
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList);
        GermanWhistPlayerItem player = PlayerList![wins];
        if (SaveRoot.WasEnd == true)
        {
            player.TricksWon++;
        }
        else if (_model!.Deck1!.IsEndOfDeck() == true)
        {
            SaveRoot.WasEnd = true;
            player.TricksWon++;
        }
        await _aTrick!.AnimateWinAsync(wins);
        if (SingleInfo!.MainHandList.Count == 0)
        {
            await GameOverAsync();
            return;
        }
        _model!.PlayerHand1!.EndTurn();
        WhoTurn = wins;
        await StartNewTrickAsync();
    }
    private async Task StartNewTrickAsync()
    {
        _aTrick!.ClearBoard();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        if (_model.Deck1!.IsEndOfDeck() == false)
        {
            GermanWhistCardInformation thisCard;
            thisCard = _model.Deck1.DrawCard();
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.Add(thisCard);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisCard.Drew = true;
                SortCards();
            }
            _command.UpdateAll();
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            GermanWhistPlayerItem player;
            if (WhoTurn == 1)
            {
                player = PlayerList[2];
            }
            else
            {
                player = PlayerList[1];
            }
            thisCard = _model.Deck1.DrawCard();
            player.MainHandList.Add(thisCard);
            _command.UpdateAll();
            if (player.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisCard.Drew = true;
                SortCards();
            }
        }
        await StartNewTurnAsync();
    }
    private async Task GameOverAsync()
    {
        SingleInfo = PlayerList.OrderByDescending(Items => Items.TricksWon).Take(1).Single();
        await ShowWinAsync();
    }
}