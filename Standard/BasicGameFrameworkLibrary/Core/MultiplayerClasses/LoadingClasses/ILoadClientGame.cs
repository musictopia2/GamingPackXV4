namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.LoadingClasses;
public interface ILoadClientGame
{
    Task LoadGameAsync(string payLoad);
}