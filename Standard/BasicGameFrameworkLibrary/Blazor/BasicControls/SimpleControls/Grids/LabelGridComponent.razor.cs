namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Grids;
public partial class LabelGridComponent
{
    [Parameter]
    [EditorRequired]
    public BasicList<LabelGridModel> Labels { get; set; } = new();
    [Parameter]
    [EditorRequired]
    public ILabelGrid? DataContext { get; set; }

    //if i need width or height, then rethink since i can't do the same way anymore.

    //[Parameter]
    //public string Width { get; set; } = "";
    //[Parameter]
    //public string Height { get; set; } = "";
    [Parameter]
    public int DecimalPlaces { get; set; } = 2;

    [Parameter]
    public int NumberDigits { get; set; } = 0;

    [Parameter]
    public EnumSize? SizeCategory { get; set; } //i think this can be default.

    [Parameter]
    public bool SuggestHeaderWidth { get; set; }
    [Parameter]
    public bool SuggestContentWidth { get; set; }


    [Parameter]
    public EnumWidth HeaderWidthCategory { get; set; }

    [Parameter]
    public EnumWidth ContentWidthCategory { get; set; }



}