namespace Countdown.Blazor.Views;
public partial class CountdownMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private CountdownGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _gameContainer = aa.Resolver!.Resolve<CountdownGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(CountdownVMData.NormalTurn))
                 .AddLabel("Round", nameof(CountdownVMData.Round))
                 .AddLabel("Status", nameof(CountdownVMData.Status));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand HintsCommand => DataContext!.HintCommand!;
}