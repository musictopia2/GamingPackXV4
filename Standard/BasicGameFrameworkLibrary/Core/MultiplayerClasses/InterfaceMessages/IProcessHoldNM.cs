namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IProcessHoldNM
{
    Task ProcessHoldReceivedAsync(int id);
}