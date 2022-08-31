namespace HitTheDeck.Blazor.Views;
public partial class HitTheDeckMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private HitTheDeckVMData? _vmData;
    private HitTheDeckGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<HitTheDeckVMData>();
        _gameContainer = aa.Resolver.Resolve<HitTheDeckGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(HitTheDeckVMData.NormalTurn))
            .AddLabel("Next", nameof(HitTheDeckVMData.NextPlayer))
            .AddLabel("Status", nameof(HitTheDeckVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(HitTheDeckPlayerItem.ObjectCount))
            .AddColumn("Total Points", true, nameof(HitTheDeckPlayerItem.TotalPoints))
            .AddColumn("Previous Points", true, nameof(HitTheDeckPlayerItem.PreviousPoints));
        base.OnInitialized();
    }
    private ICustomCommand FlipCommand => DataContext!.FlipCommand!;
    private ICustomCommand CutCommand => DataContext!.CutCommand!;
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}