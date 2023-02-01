namespace Trouble.Blazor.Views;
public partial class TroubleMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private TroubleGameContainer? _gameContainer;
    private GameBoardGraphicsCP? _graphicsData;
    protected override void OnInitialized()
    {
        _gameContainer = aa1.Resolver!.Resolve<TroubleGameContainer>();
        _graphicsData = aa1.Resolver.Resolve<GameBoardGraphicsCP>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(TroubleVMData.NormalTurn))
             .AddLabel("Instructions", nameof(TroubleVMData.Instructions))
             .AddLabel("Status", nameof(TroubleVMData.Status));
        base.OnInitialized();
    }
}