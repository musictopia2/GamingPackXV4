using BasicGameFrameworkLibrary.Core.NetworkingClasses.SocketClasses; //not common enough.
namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Misc;
public class TCPDirectSpecificIP(ITCPInfo thisTCP,
    IMessageProcessor thisMessage,
    IGamePackageResolver resolver,
    IExit exit,
    IToast toast
        ) : IGameNetwork, IServerMessage
{
    private readonly Queue<SentMessage> _messages = new();
    private readonly object _synLock = new();
    private BasicGameClientTCP? _client1;
    public bool HasServer => true;
    public Task InitAsync()
    {
        _client1 = new(this, thisTCP);
        _client1.NickName = NickName;
        return Task.CompletedTask;
    }
    public async Task<bool> InitNetworkMessagesAsync(string nickName, bool client)
    {
        bool rets;
        if (string.IsNullOrWhiteSpace(nickName))
        {
            throw new CustomBasicException("No Nick Name Upon Init Network Messages");
        }
        NickName = nickName;
        if (_client1 == null)
        {
            await InitAsync();
        }

        rets = await _client1!.ConnectToServerAsync();
        if (rets == false)
        {
            return false;
        }
        IsEnabled = true;
        if (client == false)
        {
            await _client1.HostGameAsync();
        }
        return true;
    }
    public async Task ConnectToHostAsync()
    {
        await _client1!.ConnectToHostAsync();
    }
    public async Task CloseConnectionAsync()
    {
        await _client1!.DisconnectAllAsync();
        _messages.Clear();
        _client1 = null;
    }
    public async Task SendAllAsync(string message)
    {
        SentMessage output = StartNewMessage(message);
        await SendAllAsync(output);
    }
    public async Task SendAllAsync(SentMessage tMessage)
    {
        NetworkMessage output = new();
        output.Message = await js1.SerializeObjectAsync(tMessage);
        output.YourNickName = NickName;
        await _client1!.SendMessageAsync(output);
    }
    public async Task SendSeveralSetsAsync<T>(T thisList, string finalPart)
        where T : IBasicList<string>
    {
        SentMessage output = StartNewMessage(finalPart);
        output.Body = await js1.SerializeObjectAsync(thisList);
        await SendAllAsync(output);
    }
    public async Task SendToParticularPlayerAsync(string message, string toWho) //done.
    {
        SentMessage output = StartNewMessage(message);
        await SendToParticularPlayerAsync(output, toWho);
    }
    public async Task SendToParticularPlayerAsync(SentMessage message, string toWho)
    {
        NetworkMessage output = new();
        output.SpecificPlayer = toWho;
        output.YourNickName = NickName;
        output.Message = await js1.SerializeObjectAsync(message);
        await _client1!.SendMessageAsync(output);
    }
    public static SentMessage StartNewMessage(string message)
    {
        SentMessage output = new();
        output.Status = message;
        return output;
    }
    public static SentMessage StartNewMessage(string message, string body)
    {
        SentMessage output = StartNewMessage(message);
        output.Body = body;
        return output;
    }
    private bool _isEnabled;
    public bool IsEnabled
    {
        get { return _isEnabled; }
        set
        {
            if (SetProperty(ref _isEnabled, value))
            {
                if (value == true)
                {
                    int Count;
                    lock (_synLock)
                    {
                        Count = _messages.Count;
                    }

                    if (Count > 0)
                    {

                        SentMessage CurrentMessage;
                        lock (_synLock)
                        {
                            CurrentMessage = _messages.Dequeue();
                            _ = ProcessDataAsync(CurrentMessage);
                        }
                    }
                }
            }
        }
    }
    public string NickName { get; set; } = "";
    public async Task SendToParticularPlayerAsync<T>(string status, T body, string toWho)
    {
        string News = await js1.SerializeObjectAsync(body);
        SentMessage ThisM = StartNewMessage(status, News);
        await SendToParticularPlayerAsync(ThisM, toWho);
    }
    public async Task SendToParticularPlayerAsync(string status, int data, string toWho)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString());
        await SendToParticularPlayerAsync(thisM, toWho);
    }
    public async Task SendToParticularPlayerAsync(string status, float data, string toWho)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString());
        await SendToParticularPlayerAsync(thisM, toWho);
    }
    public async Task SendToParticularPlayerAsync(string status, bool data, string toWho)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString());
        await SendToParticularPlayerAsync(thisM, toWho);
    }
    public async Task SendToParticularPlayerAsync(string status, string data, string toWho)
    {
        SentMessage thisM = StartNewMessage(status, data);
        await SendToParticularPlayerAsync(thisM, toWho);
    }
    public async Task SendAllAsync(string status, string data)
    {
        SentMessage thisM = StartNewMessage(status, data);
        await SendAllAsync(thisM);
    }
    public async Task SendAllAsync(string status, int data)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString());
        await SendAllAsync(thisM);
    }
    public async Task SendAllAsync(string status, float data)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString());
        await SendAllAsync(thisM);
    }
    public async Task SendAllAsync(string status, bool data)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString());
        await SendAllAsync(thisM);
    }
    public async Task SendAllAsync<T>(string Status, T Body)
    {
        string News = await js1.SerializeObjectAsync(Body);
        SentMessage ThisM = StartNewMessage(Status, News);
        await SendAllAsync(ThisM);
    }
    public async Task ProcessDataAsync(SentMessage thisData)
    {
        if (IsEnabled == false)
        {
            lock (_synLock)
            {
                _messages.Enqueue(thisData);
            }

            return;
        }
        IsEnabled = false;
        if (thisData.Status == "Connection Error")
        {
            await thisMessage.ProcessErrorAsync(thisData.Body);
            return;
        }
        if (thisData.Status == "hosting")
        {
            var open = resolver.Resolve<IOpeningMessenger>();
            await open.HostConnectedAsync(this);
            return;
        }
        if (thisData.Status == "clienthost")
        {
            var open = resolver.Resolve<IOpeningMessenger>();
            if (thisData.Body != "")
            {
                await open.ConnectedToHostAsync(this, thisData.Body);
                return;
            }
            toast.ShowWarningToast("Unable to connect to host.  This is TCP.  Therefore, can't keep trying");
            exit.ExitApp();
            return;
        }
        await thisMessage.ProcessMessageAsync(thisData);
    }

    public Task StartGameAsync()
    {
        return Task.CompletedTask; //tcp for now does not need to know about when a game has started.
        //for tcp, i think the host needs to connect first anyways. for tcp, if one disconnects, all has to reconnect again.
    }
    Task IGameNetwork.EndGameEarlyAsync(string nickNameReconnected)
    {
        return Task.CompletedTask; //not sure how tcp will end game early though.
    }
    public Task RestoreStateForReconnectionAsync(string nickNameReconnected)
    {
        return Task.CompletedTask;
    }
    public void ClearMessages()
    {
        lock (_synLock)
        {
            _messages.Clear();
        }
    }
    //can do direct when i do native access.
    //because wasm does not allow tcp to be used (no server for sure).  client can be iffy.
    public Task BackToMainAsync()
    {
        return Task.CompletedTask; //maybe can be ignored (?)
    }
    Task IGameNetwork.DisconnectEverybodyAsync()
    {
        return Task.CompletedTask; //not sure how tcp will disconnect everybody.
    }

    public Task NewGameAsync()
    {
        return Task.CompletedTask; //still not sure how tcp will  let everybody know about new game either.
    }
}