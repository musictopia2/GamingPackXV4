namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Interfaces;
public interface IMessageProcessor
{
    Task ProcessErrorAsync(string errorMessage); //this has to decide what to do about the network error.
    Task ProcessMessageAsync(SentMessage thisMessage);
}