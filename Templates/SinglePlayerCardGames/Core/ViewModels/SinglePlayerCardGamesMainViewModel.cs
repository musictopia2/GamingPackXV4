namespace SinglePlayerCardGames.Core.ViewModels;
[InstanceGame]
public partial class SinglePlayerCardGamesMainViewModel(IEventAggregator aggregator, CommandContainer commandContainer, IGamePackageResolver resolver)
    : IBasicEnableProcess, IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    private SinglePlayerCardGamesMainGameClass _mainGame = null!;
    public DeckObservablePile<SinglePlayerCardGamesCardInfo> DeckPile { get; set; } = null!;
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IMainGameVM.Initialize()
    {
        Subscribe();
        commandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        DeckPile = resolver.ReplaceObject<DeckObservablePile<SinglePlayerCardGamesCardInfo>>();
        DeckPile.DeckClickedAsync = DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = true;
        _mainGame = resolver.ReplaceObject<SinglePlayerCardGamesMainGameClass>();
    }

    private static async Task DeckPile_DeckClickedAsync()
    {
        //if we click on deck, will do code for this.
        await Task.CompletedTask;
    }
    private async void CommandContainer_ExecutingChanged()
    {
        if (commandContainer.IsExecuting)
        {
            return;
        }
        //code to run when its not busy.

        if (_mainGame.GameGoing)
        {
            await _mainGame.SaveStateAsync();
        }
    }
    public bool CanEnableBasics()
    {
        return true;
    }
    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        await _mainGame.NewGameAsync(DeckPile);
    }

    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        commandContainer.ExecutingChanged = null;
        Unsubscribe();
    }
}