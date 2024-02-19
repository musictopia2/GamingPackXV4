namespace GamePackageSignalRClasses;
public class SimpleClientClass
{
    public string NickName { get; set; } = "";
    private readonly ISignalRInfo _connectInfo;
    private readonly IGameInfo _gameInfo;
    private readonly IToast _toast;
    HubConnection? _hubConnection;
    private bool _isConnected;
    public event EventHandler<CustomEventHandler>? OnReceivedMessage;
    private readonly IProgress<CustomEventHandler> _thisProgress;
    public SimpleClientClass(ISignalRInfo connectInfo, IGameInfo gameInfo, IToast toast)
    {
        _connectInfo = connectInfo;
        _gameInfo = gameInfo;
        _toast = toast;
        _thisProgress = new Progress<CustomEventHandler>(items =>
        {
            OnReceivedMessage?.Invoke(this, items);
        });
    }
    public async Task<bool> ConnectToServerAsync()
    {
        if (_isConnected == true)
        {
            throw new CustomBasicException("Already connected.  Rethink");
        }
        bool isAzure = await _connectInfo.IsAzureAsync();
        int port = 0;
        if (isAzure == false)
        {
            port = await _connectInfo.GetPortAsync();
        }
        string ipAddress = await _connectInfo.GetIPAddressAsync();
        string endPoint = await _connectInfo.GetEndPointAsync(); //i do like the interface method for this.
        if (isAzure == false)
        {
            _hubConnection = new HubConnectionBuilder()
        .WithUrl($"{ipAddress}:{port}{endPoint}"
        )
        .WithAutomaticReconnect()
        .Build();
        }
        else
        {
            _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{ipAddress}{endPoint}")
            .WithAutomaticReconnect()
            .Build();
        }
        _hubConnection.On("Hosting", () =>
        {
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.Hosting));
        });
        _hubConnection.On<string>("ConnectionError", items =>
        {
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.Error, items));
        });
        _hubConnection.On<string>("HostName", items =>
        {
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.Client, items)); //this will mean the client will get the host name.
        });
        _hubConnection.On("NoHost", () =>
        {
            //maybe still okay.  because the category is none.  hopefully means will be smart enough to figure out what to do next.
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.WaitingForHost)); //this means nobody is hosting.
        });
        _hubConnection.On("NewGame", () =>
        {
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.NewGame));
        });
        _hubConnection.On("WaitForGame", () =>
        {
            //well see if the client needs to know host name in this case (?)
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.WaitingForGame));
        });
        _hubConnection.On<string>("ReceiveMessage", items =>
        {
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.Message, items));
        });

        _hubConnection.On<string>("GameState", items =>
        {
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.GameState, items));
        });
        _hubConnection.On<string>("ClientDisconnected", items =>
        {
            _toast.ShowWarningToast($"{items} was disconnected"); //hopefully this works (?)
        });
        //eventually needs to figure out the part where the host has to send 
        _hubConnection.On("Close", async () =>
        {
            await PrivateDisconnectAsync();
            //we can't do the rest automatically anymore.  because its not reliable enough.
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.CloseAll));
        });
        _hubConnection.On("EndGameEarly", () =>
        {
            //UIPlatform.ShowInfoToast("Game is ending early because the game does not support autoresume and somebody got disconnected then reconnected again");
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.EndGameEarly));
        });
        _hubConnection.On("RestoreForReconnection", () =>
        {
            _thisProgress.Report(new CustomEventHandler(EnumNetworkCategory.RestoreForReconnection));
        });
        try
        {
            await _hubConnection.StartAsync();
            _isConnected = true;
            return true; //i think this simple.
        }
        catch
        {
            return false;
        }
    }
    private async Task CheckHubConnectionAsync()
    {
        if (_hubConnection!.State != HubConnectionState.Connected)
        {
            if (_hubConnection.State == HubConnectionState.Reconnecting)
            {
                await _hubConnection.StopAsync();
            }
            int howMany = 0;
            do
            {
                try
                {
                    await _hubConnection.StartAsync();
                    break; //if you make it past this point, break out now.
                }
                catch (Exception)
                {
                    howMany++;
                    if (howMany == 1)
                    {
                        _toast.ShowWarningToast("There was a disconnection.  Will keep trying every second though.  Try to manually disconnect and reconnect again");
                    }
                    await Task.Delay(1000);
                    continue;
                }
            } while (true);
            await _hubConnection.InvokeAsync("ReconnectionAsync", NickName);
        }
    }
    private async Task PrivateDisconnectAsync()
    {
        await _hubConnection!.StopAsync();
        await _hubConnection.DisposeAsync();
        _hubConnection = null;
        _isConnected = false;
    }
    public async Task HostGameAsync()
    {
        if (NickName == "")
        {
            throw new CustomBasicException("You need to specify a nick name in order to host game");
        }
        await _hubConnection!.InvokeAsync("HostingAsync", NickName, _gameInfo.GameName); //hopefully it works.
    }
    public async Task ConnectToHostAsync()
    {
        if (NickName == "")
        {
            throw new CustomBasicException("You need to specify a nick name in order to connect to host");
        }
        await _hubConnection!.InvokeAsync("ClientConnectingAsync", NickName, _gameInfo.GameName); //i think
    }
    public async Task BackToMainAsync()
    {
        if (_isConnected == false)
        {
            return;
        }
        if (_hubConnection == null)
        {
            return;
        }
        if (NickName == "")
        {
            return; //i guess if there was no nick name, its okay.
        }
        await _hubConnection.InvokeAsync("BackToMain", NickName);
        //i don't think there is any need to do the rest this time (?)
    }
    public async Task StartGameAsync()
    {
        await _hubConnection!.InvokeAsync("StartGameAsync");
    }
    public async Task DisconnectEverybodyAsync()
    {
        await _hubConnection!.InvokeAsync("DisconnectEverybodyAsync");
    }
    public async Task NewGameAsync()
    {
        await _hubConnection!.InvokeAsync("NewGameAsync");
    }
    public async Task EndGameEarlyAsync(string nickName)
    {
        await _hubConnection!.InvokeAsync("EndGameEarlyAsync", nickName);
    }
    public async Task RestoreGameForReconnectionAsync(string nickName)
    {
        await _hubConnection!.InvokeAsync("RestoreGameForReconnectionAsync", nickName);
    }
    public async Task SendMessageAsync(NetworkMessage message)
    {
        await CheckHubConnectionAsync(); //for now only when sending messages.
        string TempMessage = await js1.SerializeObjectAsync(message);
        await _hubConnection!.InvokeAsync("SendMessageAsync", TempMessage, _gameInfo.GameName);
    }
    public async Task DisconnectAsync()
    {

        if (_isConnected == false)
        {
            return;
        }
        if (_hubConnection == null)
        {
            return;
        }
        _isConnected = false;
        await _hubConnection.InvokeAsync("CloseConnection");
        await PrivateDisconnectAsync();
    }
}