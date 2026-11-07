namespace GameLoaderBlazorLibrary;
public partial class HostedGameDiscoveryComponent(IHostedGameDiscoveryClient hosts) : IAsyncDisposable
{
    private bool _loading;
    private bool _disposed;
    private bool _cancelled;
    private bool _gameReceived;
    private string _message = "";
    private string? _pendingGame;

    [Parameter]
    public EventCallback<string> OnGameChosen { get; set; }
    [Parameter]
    public EventCallback OnCancel { get; set; }

    [Parameter]
    public string HostName { get; set; } = "";


    protected override async Task OnInitializedAsync()
    {
        hosts.HostedGameChosen += HandleGameChosenAsync;
        await StartWaitingAsync();
        await base.OnInitializedAsync();
    }

    private Task HandleGameChosenAsync(string game)
    {
        return InvokeAsync(() =>
        {
            if (_disposed || _cancelled || _gameReceived)
            {
                return;
            }

            _gameReceived = true;
            _pendingGame = game;

            // Triggers OnAfterRenderAsync to close the connection.
            StateHasChanged();
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_disposed || _cancelled || _pendingGame is null)
        {
            return;
        }

        string game = _pendingGame;
        _pendingGame = null;

        // This lifecycle method is not awaited by the SignalR callback.
        // StopWaitingAsync stops and disposes the discovery connection.
        await hosts.StopWaitingAsync();

        if (_disposed || _cancelled)
        {
            return;
        }

        await OnGameChosen.InvokeAsync(game);
    }

    private async Task StartWaitingAsync()
    {
        if (_loading)
        {
            return;
        }

        _loading = true;
        _message = "Waiting for host...";

        bool connected = await hosts.StartWaitingAsync(HostName);

        if (connected == false && !_disposed && !_cancelled && !_gameReceived)
        {
            _loading = false;
            _message = "Could not connect to the server.";
        }
    }

    private async Task CancelAsync()
    {
        _cancelled = true;
        _pendingGame = null;
        await hosts.StopWaitingAsync();

        _loading = false;
        await OnCancel.InvokeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _pendingGame = null;
        hosts.HostedGameChosen -= HandleGameChosenAsync;
        await hosts.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
