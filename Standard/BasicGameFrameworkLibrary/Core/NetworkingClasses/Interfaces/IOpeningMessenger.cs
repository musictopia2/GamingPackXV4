namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Interfaces;
public interface IOpeningMessenger
{
    Task ConnectedToHostAsync(IGameNetwork network, string hostName);
    Task WaitingForHostAsync(IGameNetwork network);
    Task HostConnectedAsync(IGameNetwork network);
    Task WaitForGameAsync(IGameNetwork network); //i like the idea that the client knows to wait for host to send game information.
}