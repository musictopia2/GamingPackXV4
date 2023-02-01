namespace LottoDominos.Blazor.Views;
public partial class MainBoardView
{
    [CascadingParameter]
    public MainBoardViewModel? DataContext { get; set; }
    private GameBoardCP? Board { get; set; }
    protected override void OnInitialized()
    {
        Board = aa1.Resolver!.Resolve<GameBoardCP>(); //best way to handle this.
        base.OnInitialized();
    }
}