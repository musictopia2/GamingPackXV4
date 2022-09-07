namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface ILoadGameNM
{
    Task LoadGameAsync(string data);
}