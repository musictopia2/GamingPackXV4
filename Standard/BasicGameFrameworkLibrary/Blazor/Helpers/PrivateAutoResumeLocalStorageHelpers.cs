namespace BasicGameFrameworkLibrary.Blazor.Helpers;
public static class PrivateAutoResumeLocalStorageHelpers
{
    //this version does not support new game except for completely reloading.
    private static async Task DeleteOldPrivateGamesAsync(string gameId) //anybody can do this.
    {
        IJSRuntime js = GetJavascript();
        await js.DeletePrivateGameAsync(gameId); //hopefully this simple.
    }
    private static IJSRuntime GetJavascript()
    {
        if (GlobalStartUp.JsRuntime is null)
        {
            throw new CustomBasicException("No javascript detected");
        }
        return GlobalStartUp.JsRuntime;
    }
    private static void AddNewGame(string gameId)
    {
        GlobalStartUp.KeysToSave.Add(gameId);
    }
    public static void RegisterPrivateAutoResumeLocalStorage()
    {
        GlobalDelegates.DeleteOldPrivateGames = DeleteOldPrivateGamesAsync;
        GlobalDelegates.AddNewGame = AddNewGame;
    }
}