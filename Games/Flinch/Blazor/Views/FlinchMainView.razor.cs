namespace Flinch.Blazor.Views;
public partial class FlinchMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private FlinchVMData? _vmData;
    private FlinchGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<FlinchVMData>();
        _gameContainer = aa.Resolver.Resolve<FlinchGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(FlinchVMData.NormalTurn))
           .AddLabel("Status", nameof(FlinchVMData.Status))
           .AddLabel("RS Cards", nameof(FlinchVMData.CardsToShuffle));
        _scores.Clear();
        _scores.AddColumn("In Stock", false, nameof(FlinchPlayerItem.InStock));
        int x;
        for (x = 1; x <= 5; x++)
        {
            var thisStr = "Discard" + x;
            _scores.AddColumn(thisStr, false, thisStr);
        }
        _scores.AddColumn("Stock Left", false, nameof(FlinchPlayerItem.StockLeft))
        .AddColumn("Cards Left", false, nameof(FlinchPlayerItem.ObjectCount));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}