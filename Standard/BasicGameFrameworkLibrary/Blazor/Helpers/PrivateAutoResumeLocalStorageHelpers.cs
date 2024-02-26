namespace BasicGameFrameworkLibrary.Blazor.Helpers;
public static class PrivateAutoResumeLocalStorageHelpers
{
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
        GlobalStartUp.KeysToSave.Add(gameId); //i think.
    }
    public static void RegisterPrivateAutoResumeLocalStorage()
    {
        GlobalDelegates.DeletePrivateGameNewRound = DeletePrivateGameNewRoundAsync;
        GlobalDelegates.DeleteOldPrivateGames = DeleteOldPrivateGamesAsync;
    }
    //once i know more details, will add on.
}