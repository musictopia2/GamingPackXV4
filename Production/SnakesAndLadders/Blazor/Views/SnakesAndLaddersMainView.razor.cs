namespace SnakesAndLadders.Blazor.Views;
public partial class SnakesAndLaddersMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private CurrentPlayerPieceModel _current = null!;
    protected override void OnInitialized()
    {
        _current = Resolver!.Resolve<CurrentPlayerPieceModel>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SnakesAndLaddersVMData.NormalTurn))
            .AddLabel("Status", nameof(SnakesAndLaddersVMData.Status));
        base.OnInitialized();
    }
}