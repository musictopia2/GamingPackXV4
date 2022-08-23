namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Interfaces;

public interface IServerMessage
{
    Task ProcessDataAsync(SentMessage thisData);
}