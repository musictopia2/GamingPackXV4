namespace BasicGameFrameworkLibrary.Blazor.LocalStorageClasses;
public class PrivateAutoResume : IPrivateSaveGame
{
    private readonly IJSRuntime _js;
    public PrivateAutoResume()
    {
        if (GlobalStartUp.JsRuntime == null)
        {
            throw new CustomBasicException("No jsruntime used");
        }
        _js = GlobalStartUp.JsRuntime;   
    }
    Task<string> IPrivateSaveGame.SavedDataAsync<T>()
    {
        if (GlobalDelegates.GetGameID is null)
        {
            throw new CustomBasicException("Nobody is getting game id");
        }
        return _js.GetPrivateGameAsync(GlobalDelegates.GetGameID());
    }
    async Task IPrivateSaveGame.SaveStateAsync<T>(T thisState)
    {
        string content = await js1.SerializeObjectAsync(thisState);
        if (GlobalDelegates.GetGameID is null)
        {
            throw new CustomBasicException("Nobody is getting game id");
        }
        await _js.UpdatePrivateGameAsync<T>(GlobalDelegates.GetGameID(), content);
    }
}