namespace SinglePlayerMiscGames.Core.ViewModels;
[InstanceGame]
public class SinglePlayerMiscGamesMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
{
    private readonly SinglePlayerMiscGamesMainGameClass _mainGame;
    public SinglePlayerMiscGamesMainViewModel(IEventAggregator aggregator, CommandContainer commandContainer, IGamePackageResolver resolver) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _mainGame = resolver.ReplaceObject<SinglePlayerMiscGamesMainGameClass>();
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
        await _mainGame.NewGameAsync();
    }
}