namespace LottoDominos.Blazor.Views;
public partial class MainBoardView
{
    private GameBoardCP? Board { get; set; }
    
    protected override void OnInitialized()
    {
        Board = Resolver!.Resolve<GameBoardCP>(); //best way to handle this.
        base.OnInitialized();
    }
}