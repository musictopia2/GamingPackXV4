namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls;

public partial class ScoreBoardBlazor<P> :
    IHandleAsync<RefreshScoreBoardEventModel<P>>, IDisposable
    where P : class, IPlayerItem, IScoreBoard, new()
{
    [Parameter]
    public PlayerCollection<P>? Players { get; set; }

    [Parameter]
    public BasicList<ScoreColumnModel> Columns { get; set; } = new();

    [Parameter]
    public string Height { get; set; } = "";

    [Parameter]
    public string Width { get; set; } = "";

    [Parameter]
    public bool UseAbbreviationForTrueFalse { get; set; }

    private readonly IEventAggregator _aggregator;
    private readonly List<P> _subscribedPlayers = new();
    private PlayerCollection<P>? _refreshedPlayers;
    private bool _disposed;

    private PlayerCollection<P>? CurrentPlayers =>
        _refreshedPlayers ?? Players;

    public ScoreBoardBlazor()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
    }

    private partial void Subscribe();
    private partial void Unsubscribe();

    protected override void OnParametersSet()
    {
        AttachPlayers();
        base.OnParametersSet();
    }

    private void DetachPlayers()
    {
        foreach (var player in _subscribedPlayers)
        {
            player.ScoreValueChanged -= ScoreChanged;
        }

        _subscribedPlayers.Clear();
    }

    private void AttachPlayers()
    {
        DetachPlayers();

        if (CurrentPlayers is null)
        {
            return;
        }

        foreach (var player in CurrentPlayers)
        {
            player.ScoreValueChanged += ScoreChanged;
            _subscribedPlayers.Add(player);
        }
    }

    private void ScoreChanged(string propertyName)
    {
        _ = InvokeAsync(() =>
        {
            if (!_disposed)
            {
                StateHasChanged();
            }
        });
    }

    Task IHandleAsync<RefreshScoreBoardEventModel<P>>.HandleAsync(
        RefreshScoreBoardEventModel<P> message)
    {
        return InvokeAsync(() =>
        {
            if (_disposed)
            {
                return;
            }

            _refreshedPlayers = message.Players;
            AttachPlayers();
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Unsubscribe();
        DetachPlayers();
        GC.SuppressFinalize(this);
    }
}