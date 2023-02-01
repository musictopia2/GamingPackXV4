namespace Sorry.Blazor.Views;
public partial class SorryMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private GameBoardGraphicsCP? _graphics;
    protected override void OnInitialized()
    {
        _graphics = aa1.Resolver!.Resolve<GameBoardGraphicsCP>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SorryVMData.NormalTurn))
                .AddLabel("Instructions", nameof(SorryVMData.Instructions))
                .AddLabel("Status", nameof(SorryVMData.Status));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}