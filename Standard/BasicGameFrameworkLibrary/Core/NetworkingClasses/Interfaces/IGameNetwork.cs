namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Interfaces;

public interface IGameNetwork
{
    Task DisconnectEverybodyAsync();
    Task BackToMainAsync();
    Task<bool> InitNetworkMessagesAsync(string nickName, bool client);
    /// <summary>
    /// This will connect to host.
    /// Since subscribe is being used, then something else will happen eventually.
    /// all this cares about is connecting to host period.
    /// whoever implements this is responsible for figuring out what happens next.
    /// </summary>
    /// <returns></returns>
    Task ConnectToHostAsync();
    Task CloseConnectionAsync();
    Task SendToParticularPlayerAsync(string message, string toWho);
    Task SendToParticularPlayerAsync<T>(string status, T content, string toWho);
    Task SendToParticularPlayerAsync(string status, string content, string toWho); //has to be more specific now.
    Task SendToParticularPlayerAsync(string status, int content, string toWho);
    Task SendToParticularPlayerAsync(string status, float content, string toWho);
    Task SendToParticularPlayerAsync(string status, bool content, string toWho);
    Task SendAllAsync(string message);
    Task SendAllAsync<T>(string status, T content);
    Task SendAllAsync(string status, string content);
    Task SendAllAsync(string status, int content);
    Task SendAllAsync(string status, float content);
    Task SendAllAsync(string status, bool content);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisList">list of messages usually sets</param>
    /// <param name="finalPart">the message identifier</param>
    /// <returns></returns>
    Task SendSeveralSetsAsync<T>(T thisList, string finalPart)
        where T : IBasicList<string>;
    //iffy now (?)
    Task StartGameAsync(); //i think that needs to notify server that the game has started.
    Task EndGameEarlyAsync(string nickNameReconnected);
    Task RestoreStateForReconnectionAsync(string nickNameReconnected);
    bool HasServer { get; } //if false, then can even ignore the multiplayer stuff.  so if somebody is using the system but has no server, then can ignore those processes.

    //i don't think we need to have as 2 different objects now.
    string NickName { get; set; } //anybody who wants to be a checker, must have a nick name.
    bool IsEnabled { get; set; } //this for sure is needed.
    Task InitAsync(); //we reserve the right for it to be async.
    void ClearMessages();
    //may have to think about how to make it where the host can disconnect and then can reconnect and make everything just work.
}