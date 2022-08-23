namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.LoadingClasses;

public interface IClientUpdateGame
{
    Task UpdateGameAsync(string payload);
}