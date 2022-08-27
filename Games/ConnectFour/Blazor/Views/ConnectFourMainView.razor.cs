namespace ConnectFour.Blazor.Views;
public partial class ConnectFourMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ConnectFourVMData.NormalTurn))
                .AddLabel("Status", nameof(ConnectFourVMData.Status));
        base.OnInitialized();
    }
    private ICustomCommand ColumnCommand => DataContext!.ColumnCommand!;

}