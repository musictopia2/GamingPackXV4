namespace Skipbo.Blazor.Views;
public partial class SkipboMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private SkipboVMData? _vmData;
    private SkipboGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<SkipboVMData>();
        _gameContainer = aa.Resolver.Resolve<SkipboGameContainer>();
        _labels.AddLabel("Turn", nameof(SkipboVMData.NormalTurn))
           .AddLabel("Status", nameof(SkipboVMData.Status))
           .AddLabel("RS Cards", nameof(SkipboVMData.CardsToShuffle));
        _scores.Clear();
        _scores.AddColumn("In Stock", false, nameof(SkipboPlayerItem.InStock));
        int x;
        for (x = 1; x <= 4; x++)
        {
            var thisStr = "Discard" + x;
            _scores.AddColumn(thisStr, false, thisStr);
        }
        _scores.AddColumn("Stock Left", false, nameof(SkipboPlayerItem.StockLeft))
        .AddColumn("Cards Left", false, nameof(SkipboPlayerItem.ObjectCount));
        base.OnInitialized();
    }
}