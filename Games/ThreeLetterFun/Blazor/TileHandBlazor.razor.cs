namespace ThreeLetterFun.Blazor;
public partial class TileHandBlazor
{
    [Parameter]
    public TileBoardObservable? Board { get; set; }
    [Parameter]
    public int TargetHeight { get; set; }
}