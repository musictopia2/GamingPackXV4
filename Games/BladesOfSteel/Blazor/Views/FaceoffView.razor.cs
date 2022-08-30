namespace BladesOfSteel.Blazor.Views;
public partial class FaceoffView
{
    [CascadingParameter]
    public BladesOfSteelVMData? VMData { get; set; }
    [CascadingParameter]
    public FaceoffViewModel? DataContext { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Instructions", nameof(BladesOfSteelVMData.Instructions));
        base.OnInitialized();
    }
}