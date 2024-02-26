namespace BasicGameFrameworkLibrary.Blazor.LocalStorageClasses;
public class PrivateAutoResume : IPrivateSaveGame
{
    private readonly IJSRuntime _js;
    private readonly IGameId _gameId;
    public PrivateAutoResume(IGameId gameId)
    {
        if (GlobalStartUp.JsRuntime == null)
        {
            throw new CustomBasicException("No jsruntime used");
        }
        _js = GlobalStartUp.JsRuntime;
        _gameId = gameId;
    }
    Task<string> IPrivateSaveGame.SavedDataAsync<T>()
    {
        return _js.GetPrivateGameAsync(_gameId.GameId);
    }
    async Task IPrivateSaveGame.SaveStateAsync<T>(T thisState)
    {
        string content = await js1.SerializeObjectAsync(thisState);
        await _js.UpdatePrivateGameAsync<T>(_gameId.GameId, content);
    }
}