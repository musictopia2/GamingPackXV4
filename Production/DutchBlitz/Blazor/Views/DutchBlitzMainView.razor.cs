namespace DutchBlitz.Blazor.Views;
public partial class DutchBlitzMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private DutchBlitzVMData? _vmData;
    private DutchBlitzGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<DutchBlitzVMData>();
        _gameContainer = Resolver.Resolve<DutchBlitzGameContainer>();
        _labels.AddLabel("Turn", nameof(DutchBlitzVMData.NormalTurn))
            .AddLabel("Status", nameof(DutchBlitzVMData.Status))
            .AddLabel("Error", nameof(DutchBlitzVMData.ErrorMessage));
        _scores.Clear();
        _scores.AddColumn("Stock Left", false, nameof(DutchBlitzPlayerItem.StockLeft))
            .AddColumn("Points Round", false, nameof(DutchBlitzPlayerItem.PointsRound))
            .AddColumn("Points Game", false, nameof(DutchBlitzPlayerItem.PointsGame));
        base.OnInitialized();
    }
    private ICustomCommand DutchCommand => DataContext!.DutchCommand!;
}