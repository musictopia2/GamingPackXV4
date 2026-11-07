namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Interfaces;
public interface IHostCatchUpNetwork
{
    Task<HostCatchUpResult> GetHostCatchUpAsync(
        string networkSessionId,
        long lastProcessedMessageSequence);

    Task<bool> TryCompleteHostCatchUpAsync(
        string networkSessionId,
        long caughtUpThroughSequence,
        string gameState);
    Task ReplayHostMessageAsync(SentMessage message);

    Task<string> BeginHostStartOverAsync();

    Task CompleteHostStartOverAsync(
        string networkSessionId,
        string gameState);

}