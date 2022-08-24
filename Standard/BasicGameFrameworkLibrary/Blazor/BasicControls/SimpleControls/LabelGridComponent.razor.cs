namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls;
public partial class LabelGridComponent
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
    public int DecimalPlaces { get; set; } = 2; //so if 0, then can do as well
    private string GetValue(LabelGridModel label)
    {
        return DataContext!.GetValue(label.Name, DecimalPlaces);
    }
}