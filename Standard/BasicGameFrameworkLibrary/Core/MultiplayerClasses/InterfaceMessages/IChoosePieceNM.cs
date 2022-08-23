namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;

public interface IChoosePieceNM
{
    Task ChoosePieceReceivedAsync(string data); //keep the option open for other possibilities.
}