namespace Xactika.Blazor.Views;
public partial class XactikaMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private XactikaVMData? _vmData;
    private XactikaGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<XactikaVMData>();
        _gameContainer = aa.Resolver.Resolve<XactikaGameContainer>();
        _labels.AddLabel("Turn", nameof(XactikaVMData.NormalTurn))
             .AddLabel("Status", nameof(XactikaVMData.Status))
             .AddLabel("Round", nameof(XactikaVMData.RoundNumber))
             .AddLabel("Mode", nameof(XactikaVMData.GameModeText));
        _scores.Clear();
        _scores.AddColumn("Cards Left", false, nameof(XactikaPlayerItem.ObjectCount))
            .AddColumn("Bid Amount", false, nameof(XactikaPlayerItem.BidAmount))
            .AddColumn("Tricks Won", false, nameof(XactikaPlayerItem.TricksWon))
            .AddColumn("Current Score", false, nameof(XactikaPlayerItem.CurrentScore))
            .AddColumn("Total Score", false, nameof(XactikaPlayerItem.TotalScore));
        base.OnInitialized();
    }
}