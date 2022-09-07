namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IPickUpNM
{
    Task PickUpReceivedAsync(string data);
}