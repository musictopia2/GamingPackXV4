namespace Battleship.Blazor.Views;
public partial class BattleshipMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private BasicList<string> _rowList = new();
    private BasicList<string> _columnList = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(BattleshipVMData.NormalTurn))
            .AddLabel("Status", nameof(BattleshipVMData.Status));
        base.OnInitialized();
    }
    private BasicList<ShipInfoCP>? _ships;
    private BattleshipCollection? _humanList;
    protected override void OnParametersSet()
    {
        var ship = aa1.Resolver!.Resolve<ShipControlCP>(); //i think.
        _ships = ship.ShipList.Values.ToBasicList();
        GameBoardCP gameBoard = aa1.Resolver.Resolve<GameBoardCP>();
        _humanList = gameBoard.HumanList!;
        _rowList = gameBoard.RowList!.Values.ToBasicList();
        _columnList = gameBoard.ColumnList!.Values.ToBasicList();
        base.OnParametersSet();
    }
    private static string ColumnText => "55vw 40vw"; //could adjust as needed.
    private string Color(bool horizontal)
    {
        if (DataContext!.ShipsHorizontal == horizontal)
        {
            return cs1.Yellow;
        }
        return cs1.Aqua;
    }
}