namespace Fluxx.Blazor;
public partial class PlayerUI
{
    [Parameter]
    public bool UsePlayerButton { get; set; }
    [CascadingParameter]
    public CompleteContainerClass? CompleteContainer { get; set; }
    [Parameter]
    public ICustomCommand? Command { get; set; }
    [Parameter]
    public string ItemHeight { get; set; } = "5vh";
    [Parameter]
    public int ItemWidth { get; set; } = 200;
}