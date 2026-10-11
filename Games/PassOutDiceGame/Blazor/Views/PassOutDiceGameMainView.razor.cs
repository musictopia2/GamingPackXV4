namespace PassOutDiceGame.Blazor.Views;
public partial class PassOutDiceGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private GameBoardGraphicsCP? _graphicsData;
    protected override void OnInitialized()
    {
        _graphicsData = Resolver!.Resolve<GameBoardGraphicsCP>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(PassOutDiceGameVMData.NormalTurn))
            .AddLabel("Status", nameof(PassOutDiceGameVMData.Status));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}