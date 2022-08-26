namespace Bingo.Blazor.Views;
public partial class BingoMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Status", nameof(BingoVMData.Status));
        base.OnInitialized();
    }
    private static string ColumnText => "1fr 1fr";
    private BingoSaveInfo? _save;
    [CascadingParameter]
    private MediaQueryListComponent? ParentElement { get; set; }
    protected override void OnParametersSet()
    {
        _save = aa.Resolver!.Resolve<BingoSaveInfo>();
        base.OnParametersSet();
    }
}