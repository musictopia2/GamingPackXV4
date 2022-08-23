namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IMoveNM
{
    Task MoveReceivedAsync(string data);
}