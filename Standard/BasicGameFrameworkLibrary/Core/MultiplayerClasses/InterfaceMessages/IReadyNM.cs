namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IReadyNM
{
    Task ProcessReadyAsync(string nickName);
}