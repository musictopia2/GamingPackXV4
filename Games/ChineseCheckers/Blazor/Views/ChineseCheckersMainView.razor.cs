namespace ChineseCheckers.Blazor.Views;
public partial class ChineseCheckersMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private GameBoardGraphicsCP? _graphics;
    private ChineseCheckersGameContainer? _container;
    protected override void OnInitialized()
    {
        _graphics = Resolver!.Resolve<GameBoardGraphicsCP>();
        _container = Resolver.Resolve<ChineseCheckersGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ChineseCheckersVMData.NormalTurn))
                .AddLabel("Instructions", nameof(ChineseCheckersVMData.Instructions))
                .AddLabel("Status", nameof(ChineseCheckersVMData.Status));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}