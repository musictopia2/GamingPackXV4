namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;

public interface IMiscDataNM
{
    Task MiscDataReceived(string status, string content);
}