namespace LifeBoardGame.Blazor.Views;
public partial class LifeBoardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private IBoardProcesses? _boardProcesses;
    private LifeBoardGameGameContainer? _gameContainer;
    private GameBoardGraphicsCP? _graphicsData;
    private LifeBoardGameVMData? _vmData;
    protected override void OnInitialized()
    {
        _boardProcesses = Resolver!.Resolve<IBoardProcesses>();
        _gameContainer = Resolver.Resolve<LifeBoardGameGameContainer>();
        _graphicsData = Resolver.Resolve<GameBoardGraphicsCP>();
        _vmData = Resolver.Resolve<LifeBoardGameVMData>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(LifeBoardGameVMData.NormalTurn))
             .AddLabel("Instructions", nameof(LifeBoardGameVMData.Instructions))
             .AddLabel("Status", nameof(LifeBoardGameVMData.Status));
        base.OnInitialized();
    }
}