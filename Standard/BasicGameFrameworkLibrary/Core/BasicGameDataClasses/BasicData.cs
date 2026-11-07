namespace BasicGameFrameworkLibrary.Core.BasicGameDataClasses;
public class BasicData
{
    public Func<int>? GetOpeningHumanPlayerCount { get; set; }
    public Func<int>? GetHumanPlayerCount { get; set; }
    public bool MultiPlayer
    {
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }
            field = value;
            ChangeState?.Invoke();
        }
    }
    public string NickName { get; set; } = "";
    //maybe can change this part in .net 7 with after binding (?)
    private bool _gameDataLoading;
    public bool GameDataLoading
    {
        get { return _gameDataLoading; }
        set
        {
            _gameDataLoading = value;
            ChangeState?.Invoke();
        }
    }
    public bool Client
    {
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }
            field = value;
            ChangeState?.Invoke();
        }
    }
    public EnumGamePackageMode GamePackageMode { get; set; } = EnumGamePackageMode.None; //default to none.  will require showing what it is.
    public Action? ChangeState { get; set; }
    public Action? DoFullScreen { get; set; }
    public bool ReloadInProgress { get; set; }
    public bool HostCatchUpPending { get; set; }
    public string NetworkSessionId { get; set; } = "";

    public int LastProcessedMessageSequence { get; set; } //my source generator did not support long (never had that).

    public SentMessage? ProcessingNetworkMessage { get; set; }

    public bool HostStartOverPending { get; set; }

}