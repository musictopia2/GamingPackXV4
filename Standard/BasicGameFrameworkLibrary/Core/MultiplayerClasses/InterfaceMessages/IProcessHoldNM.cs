namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;

public interface IProcessHoldNM
{
    Task ProcessHoldReceivedAsync(int iD); //this for sure is an integer.
}