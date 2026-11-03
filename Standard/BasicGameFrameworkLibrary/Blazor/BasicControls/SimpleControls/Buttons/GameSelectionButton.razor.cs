namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Buttons;
public partial class GameSelectionButton
{
    [Parameter]
    public EventCallback OnClick { get; set; }
    [Parameter]
    public string Display { get; set; } = "";
}