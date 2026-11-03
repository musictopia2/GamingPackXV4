namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Grids;
public partial class LabelItemComponent : IDisposable
{

    private ILabelGrid? _subscribedLabel;

    [Parameter]
    [EditorRequired]
    public ILabelGrid? DataContext { get; set; }
    [Parameter]
    public int NumberDigits { get; set; } = 0;

    [Parameter]
    [EditorRequired]
    public LabelGridModel Model { get; set; }

    [Parameter]
    [EditorRequired]
    public int DecimalPlaces { get; set; } = 2;


    [Parameter]
    [EditorRequired]
    public EnumSize? SizeCategory { get; set; } //i think this can be default.

    [Parameter]
    [EditorRequired]
    public bool SuggestHeaderWidth { get; set; }
    [Parameter]
    [EditorRequired]
    public bool SuggestContentWidth { get; set; }


    [Parameter]
    [EditorRequired]
    public EnumWidth HeaderWidthCategory { get; set; }

    [Parameter]
    [EditorRequired]
    public EnumWidth ContentWidthCategory { get; set; }

    private string GetStyle()
    {
        return $"width: {NumberDigits}ch;display: inline-block;text-align: right;font-variant-numeric: tabular-nums;";
    }

    private string GetValue(LabelGridModel label)
    {
        return DataContext!.GetValue(label.Name, DecimalPlaces);
    }

    protected override void OnParametersSet()
    {
        if (ReferenceEquals(_subscribedLabel, DataContext))
        {
            return;
        }
        _subscribedLabel?.LabelValueChanged -= LabelValueChanged;
        _subscribedLabel = DataContext;
        _subscribedLabel?.LabelValueChanged += LabelValueChanged;
    }

    private void LabelValueChanged(string propertyName)
    {

        if (propertyName == Model.Name)
        {
            InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        _subscribedLabel?.LabelValueChanged -= LabelValueChanged;
        _subscribedLabel = null;
        GC.SuppressFinalize(this);
    }

}