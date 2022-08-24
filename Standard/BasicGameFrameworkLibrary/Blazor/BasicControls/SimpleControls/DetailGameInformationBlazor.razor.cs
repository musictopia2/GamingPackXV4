namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls;
public partial class DetailGameInformationBlazor
{
    [Parameter]
    public BasicList<LabelGridModel> Labels { get; set; } = new();
    [Parameter]
    public ILabelGrid? DataContext { get; set; }
    [Parameter]
    public string Width { get; set; } = "";
    [Parameter]
    public string Height { get; set; } = "";
    [Parameter]
    public string Text { get; set; } = "Additional Information";
}