namespace TicTacToe.Blazor.Views;
public partial class TicTacToeMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private TicTacToeSaveInfo? _save;
    private WinInfo? _win;
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(TicTacToeVMData.NormalTurn))
            .AddLabel("Status", nameof(TicTacToeVMData.Status));
        base.OnInitialized();
    }
    private void GetWin()
    {
        _save = aa.Resolver!.Resolve<TicTacToeSaveInfo>();
        _win = _save.GameBoard.GetWin();
    }
    private static string GetText(SpaceInfoCP space)
    {
        return space.Status switch
        {
            EnumSpaceType.Blank => "",
            EnumSpaceType.O => "O",
            EnumSpaceType.X => "X",
            _ => "",
        };
    }
}