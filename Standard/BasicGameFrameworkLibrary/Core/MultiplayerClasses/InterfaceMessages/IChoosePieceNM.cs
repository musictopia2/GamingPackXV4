namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IChoosePieceNM
{
    Task ChoosePieceReceivedAsync(string data);
}