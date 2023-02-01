namespace TeeItUp.Blazor.Views;
public partial class TeeItUpMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private TeeItUpVMData? _vmData;
    private TeeItUpGameContainer? _gameContainer;
    private BasicList<TeeItUpPlayerItem> _players = new();
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<TeeItUpVMData>();
        _gameContainer = aa1.Resolver.Resolve<TeeItUpGameContainer>();
        if (_gameContainer.BasicData.MultiPlayer)
        {
            _players = _gameContainer!.PlayerList!.GetAllPlayersStartingWithSelf();
        }
        else
        {
            _players = _gameContainer.PlayerList!.ToBasicList();
        }
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(TeeItUpVMData.NormalTurn))
            .AddLabel("Status", nameof(TeeItUpVMData.Status))
            .AddLabel("Round", nameof(TeeItUpVMData.Round))
            .AddLabel("Instructions", nameof(TeeItUpVMData.Instructions));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(TeeItUpPlayerItem.ObjectCount))
            .AddColumn("Went Out", true, nameof(TeeItUpPlayerItem.WentOut), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Previous Score", true, nameof(TeeItUpPlayerItem.PreviousScore))
            .AddColumn("Total Score", true, nameof(TeeItUpPlayerItem.TotalScore));
        base.OnInitialized();
    }
}