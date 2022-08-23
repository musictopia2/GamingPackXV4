namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;

public interface IDiscardNM
{
    Task DiscardReceivedAsync(string data);
}