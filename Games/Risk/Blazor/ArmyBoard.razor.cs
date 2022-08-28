namespace Risk.Blazor;
public partial class ArmyBoard
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Parameter]
    public string Text { get; set; } = "";
}