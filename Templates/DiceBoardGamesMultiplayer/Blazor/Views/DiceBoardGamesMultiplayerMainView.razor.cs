namespace DiceBoardGamesMultiplayer.Blazor.Views;
public partial class DiceBoardGamesMultiplayerMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DiceBoardGamesMultiplayerVMData.NormalTurn))
             .AddLabel("Instructions", nameof(DiceBoardGamesMultiplayerVMData.Instructions))
             .AddLabel("Status", nameof(DiceBoardGamesMultiplayerVMData.Status));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
}