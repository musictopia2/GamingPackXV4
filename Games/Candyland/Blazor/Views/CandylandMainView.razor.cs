namespace Candyland.Blazor.Views;
public partial class CandylandMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private GameBoardVM? BoardModel { get; set; }
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(CandylandVMData.NormalTurn))
            .AddLabel("Status", nameof(CandylandVMData.Status));
        BoardModel = aa.Resolver!.Resolve<GameBoardVM>();
        base.OnInitialized();
    }
}