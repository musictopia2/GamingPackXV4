using BasicGameFrameworkLibrary.Core.NetworkingClasses.SocketClasses;
namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Misc;

public class TCPDirectSpecificIP : IGameNetwork, IServerMessage
{
    private readonly ITCPInfo _thisTCP;
    private readonly IMessageProcessor _thisMessage;
    private readonly IGamePackageResolver _resolver;
    private readonly IExit _exit;
    private readonly IToast _toast;
    private readonly Queue<SentMessage> _messages = new();
    private readonly object _synLock = new();
    private BasicGameClientTCP? _client1;
    public TCPDirectSpecificIP(ITCPInfo thisTCP,
        IMessageProcessor thisMessage, IGamePackageResolver resolver,
        IExit exit,
        IToast toast
        )
    {
        _thisTCP = thisTCP;
        _thisMessage = thisMessage;
        _resolver = resolver;
        _exit = exit;
        _toast = toast;
    }
    public bool HasServer => true;
    public Task InitAsync()
    {
        _client1 = new(this, _thisTCP);
        _client1.NickName = NickName;
        return Task.CompletedTask;
    }
    public async Task<bool> InitNetworkMessagesAsync(string nickName, bool client) //i think done for now for the sample.
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

        rets = await _client1!.ConnectToServerAsync(); //i think
        if (rets == false)
        {
            return false;
        }
        IsEnabled = true; //i think
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
        _messages.Clear(); //can clear all messages though.
        _client1 = null; //just get rid of it.  will just redo again.
    }
    public async Task SendAllAsync(string message)
    {
        SentMessage output = StartNewMessage(message);
        await SendAllAsync(output);
    }
    public async Task SendAllAsync(SentMessage tMessage)
    {
        NetworkMessage output = new();
        output.Message = await js.SerializeObjectAsync(tMessage);
        output.YourNickName = NickName;
        await _client1!.SendMessageAsync(output);
    }
    public async Task SendSeveralSetsAsync<T>(T thisList, string finalPart)
        where T : IBasicList<string>
    {
        SentMessage output = StartNewMessage(finalPart);
        output.Body = await js.SerializeObjectAsync(thisList);
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
        output.Message = await js.SerializeObjectAsync(message);
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
                //can decide what to do when property changes
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
    public string NickName { get; set; } = "";
    public async Task SendToParticularPlayerAsync<T>(string status, T body, string toWho)
    {
        string News = await js.SerializeObjectAsync(body);
        SentMessage ThisM = StartNewMessage(status, News);
        await SendToParticularPlayerAsync(ThisM, toWho);
    }
    public async Task SendToParticularPlayerAsync(string status, int data, string toWho)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString()); //nothing to deserialize.
        await SendToParticularPlayerAsync(thisM, toWho);
    }
    public async Task SendToParticularPlayerAsync(string status, float data, string toWho)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString()); //nothing to deserialize.
        await SendToParticularPlayerAsync(thisM, toWho);
    }
    public async Task SendToParticularPlayerAsync(string status, bool data, string toWho)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString()); //nothing to deserialize.
        await SendToParticularPlayerAsync(thisM, toWho);
    }
    public async Task SendToParticularPlayerAsync(string status, string data, string toWho)
    {
        SentMessage thisM = StartNewMessage(status, data); //nothing to deserialize.
        await SendToParticularPlayerAsync(thisM, toWho);
    }
    public async Task SendAllAsync(string status, string data)
    {
        SentMessage thisM = StartNewMessage(status, data); //try this way.
        await SendAllAsync(thisM);
    }
    public async Task SendAllAsync(string status, int data)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString()); //try this way.
        await SendAllAsync(thisM);
    }
    public async Task SendAllAsync(string status, float data)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString()); //try this way.
        await SendAllAsync(thisM);
    }
    public async Task SendAllAsync(string status, bool data)
    {
        SentMessage thisM = StartNewMessage(status, data.ToString()); //try this way.
        await SendAllAsync(thisM);
    }
    public async Task SendAllAsync<T>(string Status, T Body)
    {
        string News = await js.SerializeObjectAsync(Body);
        SentMessage ThisM = StartNewMessage(Status, News);
        await SendAllAsync(ThisM);
    }
    public async Task ProcessDataAsync(SentMessage thisData) //done.
    {
        if (IsEnabled == false) //no more bypass.
        {
            lock (_synLock)
                _messages.Enqueue(thisData);
            return;
        }
        IsEnabled = false;
        if (thisData.Status == "Connection Error")
        {
            await _thisMessage.ProcessErrorAsync(thisData.Body);
            return;
        }
        if (thisData.Status == "hosting")
        {
            var open = _resolver.Resolve<IOpeningMessenger>();
            await open.HostConnectedAsync(this);
            return;
        }
        if (thisData.Status == "clienthost")
        {
            var open = _resolver.Resolve<IOpeningMessenger>();
            if (thisData.Body != "")
            {
                await open.ConnectedToHostAsync(this, thisData.Body);
                return;
            }
            _toast.ShowWarningToast("Unable to connect to host.  This is TCP.  Therefore, can't keep trying");
            _exit.ExitApp();
            return;
        }
        await _thisMessage.ProcessMessageAsync(thisData);
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
}