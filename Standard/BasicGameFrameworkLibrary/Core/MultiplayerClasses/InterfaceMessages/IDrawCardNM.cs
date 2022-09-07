namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IDrawCardNM
{
    Task DrawCardReceivedAsync(string data);
}