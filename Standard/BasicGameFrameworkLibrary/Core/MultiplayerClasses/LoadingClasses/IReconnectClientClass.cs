namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.LoadingClasses;
public interface IReconnectClientClass
{
    Task ReconnectClientAsync(string nickName);
}