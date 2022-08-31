namespace YaBlewIt.Blazor;
public partial class SafeScreen
{
    [CascadingParameter]
    private YaBlewItMainViewModel? DataContext { get; set; }
    private static string GetColor(BasicPickerData<EnumColors> piece) => piece.EnumValue.Color; //i think.
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.AddLabel("Turn", nameof(YaBlewItVMData.OtherLabel))
            .AddLabel("Instructions", nameof(YaBlewItVMData.Instructions));
        base.OnInitialized();
    }
}