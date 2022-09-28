namespace SinglePlayerCardGames.Core.ViewModels;
[InstanceGame]
public class SinglePlayerCardGamesMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
{
    private readonly SinglePlayerCardGamesMainGameClass _mainGame;
    public DeckObservablePile<SinglePlayerCardGamesCardInfo> DeckPile { get; set; }
    public SinglePlayerCardGamesMainViewModel(IEventAggregator aggregator, CommandContainer commandContainer, IGamePackageResolver resolver) : base(aggregator)
    {
        CommandContainer = commandContainer;
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        DeckPile = resolver.ReplaceObject<DeckObservablePile<SinglePlayerCardGamesCardInfo>>();
        DeckPile.DeckClickedAsync = DeckPile_DeckClickedAsync;
        DeckPile.NeverAutoDisable = true;
        _mainGame = resolver.ReplaceObject<SinglePlayerCardGamesMainGameClass>();
    }
    private async Task DeckPile_DeckClickedAsync()
    {
        //if we click on deck, will do code for this.
        await Task.CompletedTask;
    }
    private async void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer.IsExecuting)
        {
            return;
        }
        //code to run when its not busy.

        if (_mainGame.GameGoing)
        {
            await _mainGame.SaveStateAsync();
        }
    }
    public CommandContainer CommandContainer { get; set; }
    IEventAggregator IAggregatorContainer.Aggregator => Aggregator;
    public bool CanEnableBasics()
    {
        return true;
    }
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        await _mainGame.NewGameAsync(DeckPile);
    }
}