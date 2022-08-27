namespace DominosRegular.Blazor;
public partial class DominosGameBoardBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; } //i like the idea of cascading values here.
    [Parameter]
    public GameBoardCP? DataContext { get; set; }
    private static string ViewBox()
    {
        SimpleDominoInfo domino = new();
        SizeF size = new(domino.DefaultSize.Width * 2, domino.DefaultSize.Height);
        return $"0 0 {size.Width} {size.Height}";
    }
    private string GetTargetString => TargetHeight.HeightString(); //hopefully this simple (?)
}