namespace Xactika.Blazor;
public partial class ChooseShapeBlazor
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public ChooseShapeObservable? ShapeData { get; set; }
    private string GetDisplay => ShapeData!.Visible ? "" : "none";
}
