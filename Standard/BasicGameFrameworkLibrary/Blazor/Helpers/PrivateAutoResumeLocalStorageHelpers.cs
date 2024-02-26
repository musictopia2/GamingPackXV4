namespace BasicGameFrameworkLibrary.Blazor.Helpers;
public static class PrivateAutoResumeLocalStorageHelpers
{
    //this version does not support new game except for completely reloading.
    public static async Task DeletePrivateGameNewRoundAsync(string gameId) //the client receives this message.
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
    public static async Task DeleteOldPrivateGamesAsync(string gameId)
    {
        IJSRuntime js = GetJavascript();
        await js.DeletePrivateGameAsync(gameId);
        AddNewGame(gameId);
    }
    private static void AddNewGame(string gameId)
    {
        GlobalStartUp.KeysToSave.Add(gameId);
    }
    public static void RegisterPrivateAutoResumeLocalStorage()
    {
        GlobalDelegates.DeletePrivateGameNewRound = DeletePrivateGameNewRoundAsync;
        GlobalDelegates.DeleteOldPrivateGames = DeleteOldPrivateGamesAsync;
        GlobalDelegates.AddNewGame = AddNewGame;
    }
    //once i know more details, will add on.
}