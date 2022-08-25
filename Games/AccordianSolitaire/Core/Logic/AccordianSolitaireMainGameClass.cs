namespace AccordianSolitaire.Core.Logic;
[SingletonGame]
public class AccordianSolitaireMainGameClass : RegularDeckOfCardsGameClass<AccordianSolitaireCardInfo>, IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly CommandContainer _command;
    private readonly ISystemError _error;
    private readonly IToast _toast;
    internal AccordianSolitaireSaveInfo _saveRoot;
    internal bool GameGoing { get; set; }
    public AccordianSolitaireMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IGamePackageResolver container,
        CommandContainer command,
        ISystemError error,
        IToast toast
        )
    {
        _thisState = thisState;
        Aggregator = aggregator;
        _command = command;
        _error = error;
        _toast = toast;
        _saveRoot = container.ReplaceObject<AccordianSolitaireSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
        _saveRoot.LoadMod(Aggregator);
    }
    private GameBoard? _board;
    public async Task NewGameAsync(DeckObservablePile<AccordianSolitaireCardInfo> deck, GameBoard board)
    {
        GameGoing = true;
        _board = board;
        await base.NewGameAsync(deck);
        _command.UpdateAll();
    }
    public override Task NewGameAsync(DeckObservablePile<AccordianSolitaireCardInfo> deck)
    {
        throw new CustomBasicException("Needs to use new method for newgame to send in the gameboard");
    }
    public IEventAggregator Aggregator { get; }
    public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
    {
        return await _thisState.CanOpenSavedSinglePlayerGameAsync();
    }
    public override async Task OpenSavedGameAsync()
    {
        DeckList.OrderedObjects(); //i think
        _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<AccordianSolitaireSaveInfo>();
        if (_saveRoot.DeckList.Count > 0)
        {
            var newList = _saveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
            DeckPile!.OriginalList(newList);
        }
        _saveRoot.LoadMod(Aggregator);
        _board!.ReloadSavedGame(_saveRoot);
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
        _board!.SaveGame();
        await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot); //i think
        _isBusy = false;
    }
    public async Task ShowWinAsync()
    {
        _toast.ShowSuccessToast("Congratulations, you won");
        await Task.Delay(2000);
        GameGoing = false;
        await this.SendGameOverAsync(_error);
    }
    protected override void AfterShuffle()
    {
        _saveRoot.Score = 1;
        var thisList = DeckList.ToRegularDeckDict();
        thisList.MakeAllObjectsKnown();
        _board!.NewGame(thisList, _saveRoot);
    }
}