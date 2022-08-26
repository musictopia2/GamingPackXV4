namespace BattleshipLite.Blazor.Views;
public partial class BattleshipLiteMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private BasicList<string> _rowList = new();
    private BasicList<string> _columnList = new();
    [CascadingParameter]
    private MediaQueryListComponent? Media { get; set; }
    private string GetVH()
    {
        if (Media!.DeviceCategory == EnumDeviceCategory.Phone)
        {
            return "65vh";
        }
        return "75vh";
    }
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(BattleshipLiteVMData.NormalTurn))
            .AddLabel("Status", nameof(BattleshipLiteVMData.Status))
            .AddLabel("Instructions", nameof(BattleshipLiteVMData.Instructions));
        _rowList = BattleshipBoardClass.RowList;
        _columnList = BattleshipBoardClass.ColumnList;
        base.OnInitialized();
    }
    private static string ColumnText => "55vw 40vw"; //could adjust as needed.
}