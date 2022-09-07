namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IDrewDominoNM
{
    Task DrewDominoReceivedAsync(int deck);
}