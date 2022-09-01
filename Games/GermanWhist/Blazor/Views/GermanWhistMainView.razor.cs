namespace GermanWhist.Blazor.Views;
public partial class GermanWhistMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private GermanWhistVMData? _vmData;
    private GermanWhistGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<GermanWhistVMData>();
        _gameContainer = aa.Resolver.Resolve<GermanWhistGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(GermanWhistVMData.NormalTurn))
            .AddLabel("Trump", nameof(GermanWhistVMData.TrumpSuit))
            .AddLabel("Status", nameof(GermanWhistVMData.Status));

        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(GermanWhistPlayerItem.ObjectCount))
            .AddColumn("Tricks Won", true, nameof(GermanWhistPlayerItem.TricksWon))
            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
}