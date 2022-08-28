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
        _boardProcesses = aa.Resolver!.Resolve<IBoardProcesses>();
        _gameContainer = aa.Resolver.Resolve<LifeBoardGameGameContainer>();
        _graphicsData = aa.Resolver.Resolve<GameBoardGraphicsCP>();
        _vmData = aa.Resolver.Resolve<LifeBoardGameVMData>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(LifeBoardGameVMData.NormalTurn))
             .AddLabel("Instructions", nameof(LifeBoardGameVMData.Instructions))
             .AddLabel("Status", nameof(LifeBoardGameVMData.Status));
        base.OnInitialized();
    }
}