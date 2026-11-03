namespace Xactika.Blazor;
public partial class ChooseShapeBlazor
{
    [Parameter]
    [EditorRequired]
    public double TargetHeight { get; set; } 
    [Parameter]
    public ChooseShapeObservable? ShapeData { get; set; }
    private string GetDisplay => ShapeData!.Visible ? "" : "none";
}
