namespace GamePackageSignalRClasses;
public class SignalRMessageService(ISignalRInfo thisTCP,
    IMessageProcessor thisMessage,
    IGamePackageResolver resolver,
    IGameInfo gameInfo,
    IEventAggregator aggregator,
    IToast toast
        ) : IGameNetwork //not necessarily local anymore.
{
    private readonly Queue<SentMessage> _messages = new();
    private readonly object _synLock = new();
    private SimpleClientClass? _client1;
    private IOpeningMessenger? _thisOpen;
    public string NickName { get; set; } = "";
    public bool HasServer => true;
    public Task InitAsync()
    {
        _client1 = new SimpleClientClass(thisTCP, gameInfo, toast);
        _client1.OnReceivedMessage += Client1_OnReceivedMessage!;
        _client1.NickName = NickName;
        _thisOpen = resolver.Resolve<IOpeningMessenger>();
        return Task.CompletedTask;
    }
    private async void Client1_OnReceivedMessage(object sender, CustomEventHandler e)
    {
        switch (e.Category)
        {
            case EnumNetworkCategory.WaitingForHost:
                IsEnabled = false;
                await _thisOpen!.WaitingForHostAsync(this);
                break;
            case EnumNetworkCategory.WaitingForGame:
                IsEnabled = false; //has to be false now.  because the openingviewmodel will set to true anyways.
                InProgressHelpers.Reconnecting = true;
                await _thisOpen!.WaitForGameAsync(this);
                break;
            case EnumNetworkCategory.Hosting:
                IsEnabled = false;
                await _thisOpen!.HostConnectedAsync(this);
                break;
            case EnumNetworkCategory.Client:
                IsEnabled = false;
                await _thisOpen!.ConnectedToHostAsync(this, e.Message); //i think
                break;
            case EnumNetworkCategory.CloseAll:
                IsEnabled = false; //make false
                await aggregator.PublishAsync(new DisconnectEventModel());
                break;
            //eventually need to think about other categories to be even more flexible (?)
            //throw new CustomBasicException("I don't think we will close all here.  If I am wrong, rethink");

            case EnumNetworkCategory.NewGame:
                IsEnabled = false;
                await aggregator.PublishAsync(new ClientNewGameEventModel());
                break;
            case EnumNetworkCategory.Message:
                SentMessage data = await js1.DeserializeObjectAsync<SentMessage>(e.Message);
                await ProcessDataAsync(data);
                break;
            case EnumNetworkCategory.GameState:
                //no open this time.
                //i propose sending an event model message.
                await aggregator.PublishAsync(new ReconnectEventModel(e.Message));
                break;
            case EnumNetworkCategory.Error:
                IsEnabled = false;
                await thisMessage.ProcessErrorAsync(e.Message);
                break;
            case EnumNetworkCategory.EndGameEarly:
                ClearMessages();
                IsEnabled = true; //has to be enabled.
                InProgressHelpers.Reconnecting = true;
                toast.ShowInfoToast("Game is ending early because the game does not support autoresume and somebody got disconnected then reconnected again");
                await aggregator.PublishAsync(new EndGameEarlyEventModel()); //so can do any remaining cleanup.  anybody who needs to listen and do something can do.
                break;
            case EnumNetworkCategory.RestoreForReconnection:
                ClearMessages();
                IsEnabled = true; //needs to be true so can receive the next message which needs to be to reconnect.
                InProgressHelpers.Reconnecting = true;
                toast.ShowInfoToast("Game is being restored which means redoing turns in progress since body got disconnected then reconnected again");
                //hopefully nothing else is needed (?)
                break;
            default:
                break;
        }
    }
    public async Task ProcessDataAsync(SentMessage thisData) //done.
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
        await thisMessage.ProcessMessageAsync(thisData);
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
        rets = await _client1!.ConnectToServerAsync();
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
        await _client1!.DisconnectAsync();
        _messages.Clear(); //can clear all messages though.
        _client1 = null!; //just get rid of it.  will just redo again.
    }
    public async Task SendAllAsync(string message)
    {
        SentMessage output = StartNewMessage(message);
        await SendAllAsync(output);
    }
    public async Task SendAllAsync(SentMessage tmessage)
    {
        NetworkMessage output = new();
        output.Message = await js1.SerializeObjectAsync(tmessage);
        output.YourNickName = NickName;
        await _client1!.SendMessageAsync(output);
    }
    public async Task SendSeveralSetsAsync<T>(T thisList, string finalPart)
        where T: IBasicList<string>
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
                        SentMessage currentMessage;
                        lock (_synLock)
                        {
                            currentMessage = _messages.Dequeue(); //try this.
                        }
                        _ = ProcessDataAsync(currentMessage);
                    }
                }
            }
        }
    }
    public async Task SendToParticularPlayerAsync(string status, string data, string toWho)
    {
        SentMessage thisM = StartNewMessage(status, data); //nothing to deserialize.
        await SendToParticularPlayerAsync(thisM, toWho);
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
    public async Task SendToParticularPlayerAsync<T>(string status, T data, string toWho)
    {
        string news = await js1.SerializeObjectAsync(data);
        SentMessage thisM = StartNewMessage(status, news);
        await SendToParticularPlayerAsync(thisM, toWho);
    }
    public async Task SendAllAsync<T>(string status, T data)
    {
        string news = await js1.SerializeObjectAsync(data);
        SentMessage thisM = StartNewMessage(status, news);
        await SendAllAsync(thisM);
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
    public async Task StartGameAsync()
    {
        await _client1!.StartGameAsync();
    }
    public async Task EndGameEarlyAsync(string nickNameReconnected)
    {
        await _client1!.EndGameEarlyAsync(nickNameReconnected);
    }
    public async Task RestoreStateForReconnectionAsync(string nickNameReconnected)
    {
        await _client1!.RestoreGameForReconnectionAsync(nickNameReconnected);
    }
    public void ClearMessages()
    {
        lock (_synLock)
        {
            _messages.Clear();
        }
    }
    public async Task BackToMainAsync()
    {
        if (_client1 is null)
        {
            return; //because nothing was done anyways.  this will mean will not require internet access for future.
        }
        await _client1.BackToMainAsync();
    }
    async Task IGameNetwork.DisconnectEverybodyAsync()
    {
        await _client1!.DisconnectEverybodyAsync();
    }
    public async Task NewGameAsync()
    {
        await _client1!.NewGameAsync();
    }
}