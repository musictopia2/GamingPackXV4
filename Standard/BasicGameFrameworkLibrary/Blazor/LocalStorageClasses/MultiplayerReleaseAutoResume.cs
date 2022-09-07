namespace BasicGameFrameworkLibrary.Blazor.LocalStorageClasses;
public class MultiplayerReleaseAutoResume : IMultiplayerSaveState
{
    private readonly IGameInfo _game;
    private readonly BasicData _data;
    private readonly TestOptions _test;
    private readonly string _singleName;
    private readonly string _multiName;
    private readonly IJSRuntime _js;
    public MultiplayerReleaseAutoResume(IGameInfo game, BasicData data, TestOptions test)
    {
        _game = game;
        _data = data;
        _test = test;
        _singleName = $"{game.GameName} Release Single";
        _multiName = $"{game.GameName} Release Multiplayer";
        if (GlobalStartUp.JsRuntime == null)
        {
            throw new CustomBasicException("No jsruntime used");
        }
        _js = GlobalStartUp.JsRuntime;
    }
    private string GetCurrentName()
    {
        string name;
        if (_data.MultiPlayer == false)
        {
            name = _singleName;
        }
        else
        {
            name = _multiName;
        }
        return name;
    }
    async Task IMultiplayerSaveState.DeleteGameAsync()
    {
        await DeleteGameAsync();
    }
    private async Task DeleteGameAsync()
    {
        if (CanChange() == false)
        {
            return;
        }
        string name = GetCurrentName();
        if (_js.ContainsKey(name) == false)
        {
            return;
        }
        await _js.StorageRemoveItemAsync(name);
    }
    async Task IMultiplayerSaveState.DeleteMultiplayerGameAsync()
    {
        await DeleteGameAsync();
    }
    async Task<EnumRestoreCategory> IMultiplayerSaveState.MultiplayerRestoreCategoryAsync()
    {
        await Task.CompletedTask;
        if (_test.SaveOption == EnumTestSaveCategory.NoSave)
        {
            return EnumRestoreCategory.NoRestore;
        }
        bool rets = _js.ContainsKey(_multiName);
        if (rets == false)
        {
            return EnumRestoreCategory.NoRestore;
        }
        if (_test.SaveOption == EnumTestSaveCategory.RestoreOnly)
        {
            return EnumRestoreCategory.MustRestore;
        }
        return EnumRestoreCategory.CanRestore;
    }
    async Task<EnumRestoreCategory> IMultiplayerSaveState.SinglePlayerRestoreCategoryAsync()
    {
        await Task.CompletedTask;
        if (_test.SaveOption == EnumTestSaveCategory.NoSave)
        {
            return EnumRestoreCategory.NoRestore;
        }
        bool rets = _js.ContainsKey(_singleName);
        if (rets == false)
        {
            return EnumRestoreCategory.NoRestore;
        }
        if (_test.SaveOption == EnumTestSaveCategory.RestoreOnly)
        {
            return EnumRestoreCategory.MustRestore;
        }
        return EnumRestoreCategory.CanRestore;
    }
    async Task<string> IMultiplayerSaveState.SavedDataAsync<T>()
    {
        if (CanChange() == false)
        {
            return "";
        }
        string name = GetCurrentName();
        if (_js.ContainsKey(name) == false)
        {
            return "";
        }
        try
        {
            string output = await _js.StorageGetStringAsync(name);
            return output;
        }
        catch (Exception)
        {
            return "";
        }
    }
    private bool CanChange()
    {
        if (_game.CanAutoSave == false || _test.SaveOption != EnumTestSaveCategory.Normal)
        {
            return false;
        }
        return true;
    }
    async Task IMultiplayerSaveState.SaveStateAsync<T>(T thisState)
    {
        if (CanChange() == false)
        {
            return;
        }
        await Task.Delay(5);
        string name = GetCurrentName();
        string content = await js.SerializeObjectAsync(thisState);
        await _js.UpdateLocalStorageAsync(name, content);
    }
    async Task<string> IMultiplayerSaveState.TempMultiSavedAsync()
    {
        if (CanChange() == false)
        {
            return "";
        }
        if (_js.ContainsKey(_multiName) == false)
        {
            return "";
        }
        try
        {
            string output = await _js.StorageGetStringAsync(_multiName);
            return output;
        }
        catch (Exception)
        {
            return "";
        }
    }
}