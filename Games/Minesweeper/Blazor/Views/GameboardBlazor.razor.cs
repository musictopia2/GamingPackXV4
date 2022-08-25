namespace Minesweeper.Blazor.Views;
public partial class GameboardBlazor
{
    [CascadingParameter]
    public MinesweeperMainViewModel? DataContext { get; set; }
    [Parameter]
    public Action? StateChanged { get; set; }
    private MinesweeperMainGameClass MainGame { get; set; }
    public GameboardBlazor()
    {
        MainGame = aa.Resolver!.Resolve<MinesweeperMainGameClass>();
    }
    private BasicList<MineSquareModel> SquareList => MainGame.GetSquares();
    public string GetViewHeight()
    {
        double totalNeeded = 90 / MainGame.NumberOfRows;
        return $"{totalNeeded}vh";
    }
    private MineSquareModel GetSquare(int row, int column)
    {
        return SquareList.Single(x => x.Row == row && x.Column == column);
    }
}