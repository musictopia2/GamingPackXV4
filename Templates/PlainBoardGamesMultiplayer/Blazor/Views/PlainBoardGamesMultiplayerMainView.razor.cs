namespace PlainBoardGamesMultiplayer.Blazor.Views;
public partial class PlainBoardGamesMultiplayerMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(PlainBoardGamesMultiplayerVMData.NormalTurn))
                .AddLabel("Instructions", nameof(PlainBoardGamesMultiplayerVMData.Instructions))
                .AddLabel("Status", nameof(PlainBoardGamesMultiplayerVMData.Status));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}