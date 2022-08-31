namespace YaBlewIt.Blazor.Views;
public partial class YaBlewItMainView
{
    private bool _showPopup;
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private YaBlewItVMData? _vmData;
    private YaBlewItGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<YaBlewItVMData>();
        _gameContainer = aa.Resolver.Resolve<YaBlewItGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Prospector", nameof(YaBlewItVMData.NormalTurn))
            .AddLabel("Other Turn", nameof(YaBlewItVMData.OtherLabel))
            .AddLabel("Instructions", nameof(YaBlewItVMData.Instructions))
            .AddLabel("Status", nameof(YaBlewItVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards In Hand", true, nameof(YaBlewItPlayerItem.ObjectCount))
            .AddColumn("Cursed Gem", true, nameof(YaBlewItPlayerItem.CursedGem))
            .AddColumn("Miner In Round", true, nameof(YaBlewItPlayerItem.InGame), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Total Score", true, nameof(YaBlewItPlayerItem.TotalScore))
            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand PlayCommand => DataContext!.PlayCardCommand!;
    private ICustomCommand PassCommand => DataContext!.PassCommand!;
    private ICustomCommand TakeClaimCommand => DataContext!.TakeClaimCommand!;
    private ICustomCommand GambleCommand => DataContext!.GambleCommand!;
    private void OpenPopup()
    {
        _showPopup = true;
    }
    private void ClosePopup()
    {
        _showPopup = false;
    }
}