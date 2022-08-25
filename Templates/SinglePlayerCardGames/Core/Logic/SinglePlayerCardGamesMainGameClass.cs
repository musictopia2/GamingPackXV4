namespace SinglePlayerCardGames.Core.Logic;
[SingletonGame]
public class SinglePlayerCardGamesMainGameClass : RegularDeckOfCardsGameClass<SinglePlayerCardGamesCardInfo>, IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly IMessageBox _message;
    private readonly ISystemError _error;
    internal SinglePlayerCardGamesSaveInfo _saveRoot;
    public SinglePlayerCardGamesMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IGamePackageResolver container,
        IMessageBox message,
        ISystemError error
        )
    {
        _thisState = thisState;
        Aggregator = aggregator;
        _message = message;
        _error = error;
        _saveRoot = container.ReplaceObject<SinglePlayerCardGamesSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
    }
    internal bool GameGoing { get; set; }
    public IEventAggregator Aggregator { get; }
    public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
    {
        return await _thisState.CanOpenSavedSinglePlayerGameAsync();
    }
    public override async Task OpenSavedGameAsync()
    {
        DeckList.OrderedObjects(); //i think
        _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<SinglePlayerCardGamesSaveInfo>();
        if (_saveRoot.DeckList.Count > 0)
        {
            var newList = _saveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
            DeckPile!.OriginalList(newList);
        }
        //anything else that is needed to open the saved game will be here.

    }
    public override Task NewGameAsync(DeckObservablePile<SinglePlayerCardGamesCardInfo> deck)
    {
        GameGoing = true;
        return base.NewGameAsync(deck);
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
        GameGoing = false;
        await _message.ShowMessageAsync("You Win");
        await _thisState.DeleteSinglePlayerGameAsync();
        //send message to show win.
        await this.SendGameOverAsync(_error);
    }
    protected override void AfterShuffle()
    {
        //this is what runs after the cards shuffle.

    }
}