namespace BasicGameFrameworkLibrary.Blazor.LocalStorageClasses;
public class PrivateAutoResume<P, S> : IPrivateSaveGame
    where P : class, IPlayerItem, new()
    where S : BasicSavedGameClass<P>, new()
{
    private IGameSetUp<P, S>? _gameSetUp;
    private readonly IJSRuntime _js;
    public PrivateAutoResume(IGameSetUp<P, S> gameSetUp)
    {
        _gameSetUp = gameSetUp;
        if (GlobalStartUp.JsRuntime == null)
        {
            throw new CustomBasicException("No jsruntime used");
        }
        _js = GlobalStartUp.JsRuntime;
    }
    Task<string> IPrivateSaveGame.SavedDataAsync<T>()
    {
        return _js.GetPrivateGameAsync(_gameSetUp!.SaveRoot);
    }
    async Task IPrivateSaveGame.SaveStateAsync<T>(T thisState)
    {
        string content = await js1.SerializeObjectAsync(thisState);
        await _js.UpdatePrivateGameAsync(_gameSetUp!.SaveRoot, content);
    }
}