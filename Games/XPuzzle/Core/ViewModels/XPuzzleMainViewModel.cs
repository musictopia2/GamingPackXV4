namespace XPuzzle.Core.ViewModels;
[InstanceGame]
public class XPuzzleMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
{
    private readonly XPuzzleMainGameClass _mainGame;
    public XPuzzleMainViewModel(IEventAggregator aggregator, CommandContainer commandContainer, IGamePackageResolver resolver) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _mainGame = resolver.ReplaceObject<XPuzzleMainGameClass>();
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