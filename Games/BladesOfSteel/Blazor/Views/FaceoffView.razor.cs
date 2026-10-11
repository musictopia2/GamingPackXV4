namespace BladesOfSteel.Blazor.Views;
public partial class FaceoffView
{
    [CascadingParameter]
    public BladesOfSteelVMData? VMData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Instructions", nameof(BladesOfSteelVMData.Instructions));
        base.OnInitialized();
    }


    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            VMData!.Deck1.ReportCanExecuteChange();
        }
    }

}