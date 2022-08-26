namespace SnakesAndLadders.Blazor.Views;
public partial class SnakesAndLaddersMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SnakesAndLaddersVMData.NormalTurn))
            .AddLabel("Status", nameof(SnakesAndLaddersVMData.Status));
        base.OnInitialized();
    }
}