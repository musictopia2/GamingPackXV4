namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IEndTurnNM
{
    Task EndTurnReceivedAsync(string data);
}