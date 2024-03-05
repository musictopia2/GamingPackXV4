namespace SorryDicedGame.Blazor.Views;
public partial class SorryDicedGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SorryDicedGameVMData.NormalTurn))
                .AddLabel("Instructions", nameof(SorryDicedGameVMData.Instructions))
                .AddLabel("Status", nameof(SorryDicedGameVMData.Status));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}