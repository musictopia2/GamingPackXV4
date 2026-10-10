namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls;
public partial class LabelItemComponent : IDisposable
{

    private ILabelGrid? _subscribedLabel;

    [Parameter]
    [EditorRequired]
    public ILabelGrid? DataContext { get; set; }

    [Parameter]
    [EditorRequired]
    public LabelGridModel Model { get; set; }

    [Parameter]
    [EditorRequired]
    public int DecimalPlaces { get; set; } = 2;

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