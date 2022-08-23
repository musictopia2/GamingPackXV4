namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;

public interface ITrickNM
{
    Task TrickPlayReceivedAsync(int deck);
}