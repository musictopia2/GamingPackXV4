namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Frames;
public partial class DetailGameInformationBlazor
{
    [Parameter]
    public BasicList<LabelGridModel> Labels { get; set; } = new();
    [Parameter]
    public ILabelGrid? DataContext { get; set; }
    [Parameter]
    public bool SuggestHeaderWidth { get; set; }
    [Parameter]
    public bool SuggestContentWidth { get; set; }


    [Parameter]
    public EnumWidth HeaderWidthCategory { get; set; }

    [Parameter]
    public EnumWidth ContentWidthCategory { get; set; }
    [Parameter]
    public string Text { get; set; } = "Additional Information";
}