namespace ClockSolitaire.Core.Logic;
[SingletonGame]
public class ClockSolitaireMainGameClass : RegularDeckOfCardsGameClass<SolitaireCard>, IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly IToast _toast;
    private readonly ISystemError _error;
    private readonly CommandContainer _command;
    internal ClockSolitaireSaveInfo SaveRoot { get; set; }
    internal bool GameGoing { get; set; }
    public ClockSolitaireMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IGamePackageResolver container,
        IToast toast,
        ISystemError error,
        CommandContainer command
        )
    {
        Aggregator = aggregator;
        _toast = toast;
        _error = error;
        _command = command;
        _thisState = thisState;
        SaveRoot = container.ReplaceObject<ClockSolitaireSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
        SaveRoot.LoadMod(aggregator);
    }
    private ClockBoard? _clock1;
    public async Task NewGameAsync(ClockSolitaireMainViewModel model)
    {
        GameGoing = true;
        _clock1 = model.Clock1;
        await base.NewGameAsync(model.DeckPile);
    }
    public override Task NewGameAsync(DeckObservablePile<SolitaireCard> deck)
    {
        throw new CustomBasicException("Must new new function");
    }
    public IEventAggregator Aggregator { get; }
    public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
    {
        return await _thisState.CanOpenSavedSinglePlayerGameAsync();
    }
    public override async Task OpenSavedGameAsync()
    {
        DeckList.OrderedObjects(); //i think
        SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<ClockSolitaireSaveInfo>();
        if (SaveRoot.DeckList.Count > 0)
        {
            var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
            DeckPile!.OriginalList(newList);
        }
        _clock1!.LoadSavedClocks(SaveRoot.SavedClocks);
        SaveRoot.LoadMod(Aggregator);
    }
    private bool _isBusy;
    public async Task SaveStateAsync()
    {
        if (_isBusy)
        {
            return;
        }
        _isBusy = true;
        SaveRoot.DeckList = DeckPile!.GetCardIntegers();
        _clock1!.SaveGame();
        await _thisState.SaveSimpleSinglePlayerGameAsync(SaveRoot);
        _isBusy = false;
    }
    public async Task ShowWinAsync()
    {
        _command.UpdateAll();
        _toast.ShowSuccessToast("Congratulations, you won");
        await Task.Delay(2000);
        GameGoing = false;
        await this.SendGameOverAsync(_error);
    }
    public async Task ShowLossAsync()
    {
        _command.UpdateAll();
        _toast.ShowWarningToast("Sorry, you lost");
        await Task.Delay(2000);
        GameGoing = false;
        await this.SendGameOverAsync(_error);
    }
    protected override void AfterShuffle()
    {
        _clock1!.NewGame(DeckList.ToRegularDeckDict());
    }
}