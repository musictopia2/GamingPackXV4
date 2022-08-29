namespace Aggravation.Blazor.Views;
public partial class AggravationMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private AggravationGameContainer? _gameContainer;
    private GameBoardGraphicsCP? _graphicsData;
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(AggravationVMData.NormalTurn))
             .AddLabel("Instructions", nameof(AggravationVMData.Instructions))
             .AddLabel("Status", nameof(AggravationVMData.Status));
        _gameContainer = aa.Resolver!.Resolve<AggravationGameContainer>();
        _graphicsData = aa.Resolver.Resolve<GameBoardGraphicsCP>();
        base.OnInitialized();
    }
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}