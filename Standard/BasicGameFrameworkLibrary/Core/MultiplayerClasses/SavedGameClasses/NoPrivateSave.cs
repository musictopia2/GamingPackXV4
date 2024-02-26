namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;
public class NoPrivateSave : IPrivateSaveGame
{
    Task<string> IPrivateSaveGame.SavedDataAsync<T>()
    {
        return Task.FromResult("");
    }

    Task IPrivateSaveGame.SaveStateAsync<T>(T thisState)
    {
        return Task.CompletedTask;
    }
}