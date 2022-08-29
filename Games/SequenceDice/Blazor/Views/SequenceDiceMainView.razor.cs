namespace SequenceDice.Blazor.Views;
public partial class SequenceDiceMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private SequenceDiceSaveInfo? _saveRoot;
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SequenceDiceVMData.NormalTurn))
             .AddLabel("Instructions", nameof(SequenceDiceVMData.Instructions))
             .AddLabel("Status", nameof(SequenceDiceVMData.Status));
        base.OnInitialized();
    }
    protected override void OnParametersSet()
    {
        _saveRoot = aa.Resolver!.Resolve<SequenceDiceSaveInfo>();
        base.OnParametersSet();
    }
    private static string ColumnText => "50vw 50vw"; //could adjust as needed.
    private ICustomCommand MoveCommand => DataContext!.MakeMoveCommand!;
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}