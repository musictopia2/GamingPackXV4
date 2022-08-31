namespace DutchBlitz.Blazor.Views;
public partial class DutchBlitzMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private DutchBlitzVMData? _vmData;
    private DutchBlitzGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<DutchBlitzVMData>();
        _gameContainer = aa.Resolver.Resolve<DutchBlitzGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DutchBlitzVMData.NormalTurn))
            .AddLabel("Status", nameof(DutchBlitzVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(DutchBlitzPlayerItem.ObjectCount))

            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
}