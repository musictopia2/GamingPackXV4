namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IRolledNM
{
    Task RollReceivedAsync(string data);
}