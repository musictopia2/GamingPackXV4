namespace BasicGameFrameworkLibrary.Blazor.LocalStorageClasses;

public class SinglePlayerReleaseAutoResume(IGameInfo game) : ISaveSinglePlayerClass
{
    private readonly IJSRuntime _js = GlobalStartUp.JsRuntime
            ?? throw new CustomBasicException("No jsruntime used");
    private readonly string _storageKey = $"{game.GameName} Release Single";

    Task<bool> ISaveSinglePlayerClass.CanOpenSavedSinglePlayerGameAsync()
    {
        return Task.FromResult(
            game.CanAutoSave && _js.ContainsKey(_storageKey));
    }

    async Task ISaveSinglePlayerClass.DeleteSinglePlayerGameAsync()
    {
        if (game.CanAutoSave == false ||
            _js.ContainsKey(_storageKey) == false)
        {
            return;
        }

        await _js.StorageRemoveItemAsync(_storageKey);
    }

    async Task<T> ISaveSinglePlayerClass.RetrieveSinglePlayerGameAsync<T>()
    {
        if (game.CanAutoSave == false ||
            _js.ContainsKey(_storageKey) == false)
        {
            throw new CustomBasicException(
                "No saved single player game is available.");
        }

        string content = await _js.StorageGetStringAsync(_storageKey);
        return await js1.DeserializeObjectAsync<T>(content);
    }

    async Task ISaveSinglePlayerClass.SaveSimpleSinglePlayerGameAsync<T>(
        T thisObject)
    {
        if (game.CanAutoSave == false)
        {
            return;
        }

        if (thisObject is null)
        {
            throw new CustomBasicException("Cannot save null object.");
        }

        string content = await js1.SerializeObjectAsync(thisObject);
        await _js.UpdateLocalStorageAsync(_storageKey, content);
    }
}