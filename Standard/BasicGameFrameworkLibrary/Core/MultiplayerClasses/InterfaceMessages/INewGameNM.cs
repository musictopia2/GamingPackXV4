namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;

public interface INewGameNM
{
    Task NewGameReceivedAsync(string data);
}