namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Interfaces;
public interface IHostedGameDiscoveryClient : IAsyncDisposable
{
    event Func<string, Task>? HostedGameChosen;
    Task<bool> StartWaitingAsync(string hostName = "");
    Task StopWaitingAsync();
}