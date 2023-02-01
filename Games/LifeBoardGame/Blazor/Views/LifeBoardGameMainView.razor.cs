namespace LifeBoardGame.Blazor.Views;
public partial class LifeBoardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private IBoardProcesses? _boardProcesses;
    private LifeBoardGameGameContainer? _gameContainer;
    private GameBoardGraphicsCP? _graphicsData;
    private LifeBoardGameVMData? _vmData;
    protected override void OnInitialized()
    {
        _boardProcesses = aa1.Resolver!.Resolve<IBoardProcesses>();
        _gameContainer = aa1.Resolver.Resolve<LifeBoardGameGameContainer>();
        _graphicsData = aa1.Resolver.Resolve<GameBoardGraphicsCP>();
        _vmData = aa1.Resolver.Resolve<LifeBoardGameVMData>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(LifeBoardGameVMData.NormalTurn))
             .AddLabel("Instructions", nameof(LifeBoardGameVMData.Instructions))
             .AddLabel("Status", nameof(LifeBoardGameVMData.Status));
        base.OnInitialized();
    }
}