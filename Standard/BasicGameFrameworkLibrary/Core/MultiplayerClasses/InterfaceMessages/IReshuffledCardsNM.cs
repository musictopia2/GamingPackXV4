namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IReshuffledCardsNM
{
    Task ReshuffledCardsReceived(string data);
}