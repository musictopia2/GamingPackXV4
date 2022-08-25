
namespace SolitaireBoardGame.Core.ViewModels;
[InstanceGame]
public partial class SolitaireBoardGameMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
{
    private readonly SolitaireBoardGameMainGameClass _mainGame;
    private readonly BasicData _basicData;
    public SolitaireBoardGameMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        BasicData basicData) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        _mainGame = resolver.ReplaceObject<SolitaireBoardGameMainGameClass>();
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    IEventAggregator IAggregatorContainer.Aggregator => Aggregator;
    public bool CanEnableBasics()
    {
        return true;
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = true;
        await base.ActivateAsync();
        await _mainGame.NewGameAsync();
        _basicData.GameDataLoading = false;
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task MakeMoveAsync(GameSpace space)
    {
        await _mainGame.ProcessCommandAsync(space);
    }
}