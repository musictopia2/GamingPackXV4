namespace DutchBlitz.Blazor.Views;
public partial class DutchBlitzMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private DutchBlitzVMData? _vmData;
    private DutchBlitzGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<DutchBlitzVMData>();
        _gameContainer = aa1.Resolver.Resolve<DutchBlitzGameContainer>();
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