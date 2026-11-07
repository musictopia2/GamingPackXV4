namespace GamePackageSignalRClasses;
public class HostedGameDiscoveryClient(ISignalRInfo thisTCP)
    : IHostedGameDiscoveryClient
{
    private HubConnection? _hubConnection;
    private int _receivedGame;
    private bool _disposed;
    public event Func<string, Task>? HostedGameChosen;
    public async Task<bool> StartWaitingAsync(string hostName = "")
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // Close any previous attempt before starting again.
        await StopWaitingAsync();
        _receivedGame = 0;

        int? port = await thisTCP.GetPortAsync();
        string address = await thisTCP.GetIPAddressAsync();
        string endpoint = await thisTCP.GetEndPointAsync();

        string url = port.HasValue
            ? $"{address}:{port.Value}{endpoint}"
            : $"{address}{endpoint}";

        HubConnection connection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();

        _hubConnection = connection;

        // Register before requesting the game.
        connection.On<string>("HostedGameChosen", async game =>
        {
            if (string.IsNullOrWhiteSpace(game))
            {
                return;
            }

            if (Interlocked.Exchange(ref _receivedGame, 1) != 0)
            {
                return;
            }

            var handler = HostedGameChosen;
            if (handler is not null)
            {
                await handler.Invoke(game);
            }
        });

        try
        {
            await connection.StartAsync();

            if (string.IsNullOrWhiteSpace(hostName))
            {
                await connection.InvokeAsync("WaitForHostedGameAsync");
            }
            else
            {
                await connection.InvokeAsync(
                    "WaitForNextHostedGameAsync", hostName);
            }

            return true;
        }
        catch
        {
            await StopWaitingAsync();
            return false;
        }
    }

    public async Task StopWaitingAsync()
    {
        HubConnection? connection = _hubConnection;
        _hubConnection = null;

        if (connection is null)
        {
            return;
        }

        try
        {
            await connection.StopAsync();
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        await StopWaitingAsync();

        GC.SuppressFinalize(this);
    }
}