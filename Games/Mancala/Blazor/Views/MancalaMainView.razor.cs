namespace Mancala.Blazor.Views;
public partial class MancalaMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private GameBoardVM? OtherModel { get; set; }
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(MancalaVMData.NormalTurn))
            .AddLabel("Status", nameof(MancalaVMData.Status))
            .AddLabel("Instructions", nameof(MancalaVMData.Instructions));
        base.OnInitialized();
    }
    protected override void OnParametersSet()
    {
        OtherModel = aa1.Resolver!.Resolve<GameBoardVM>();
        base.OnParametersSet();
    }
}