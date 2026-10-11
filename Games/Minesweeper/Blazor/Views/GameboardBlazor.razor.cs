namespace Minesweeper.Blazor.Views;
public partial class GameboardBlazor : IHandle<BeforeGameOverEventModel>, IDisposable
{
    [CascadingParameter]
    public MinesweeperMainViewModel? DataContext { get; set; }
    [Parameter]
    public Action? StateChanged { get; set; }
    private MinesweeperMainGameClass MainGame { get; set; }
    private readonly IEventAggregator _aggregator;
    private partial void Subscribe();
    private partial void Unsubscribe();
    public GameboardBlazor()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        MainGame = Resolver!.Resolve<MinesweeperMainGameClass>();
        Subscribe();
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

    void IHandle<BeforeGameOverEventModel>.Handle(BeforeGameOverEventModel message)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
}