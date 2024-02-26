namespace BasicGameFrameworkLibrary.Blazor.Extensions;
public static class GlobalSettingsExtensions
{
    public static async Task LoadGlobalDataAsync(this IJSRuntime js)
    {
        GlobalDataModel output;
        bool rets;
        rets = await js.ContainsKeyAsync(GlobalDataModel.LocalStorageKey); //has to use async version in order to work with wpf.
        if (rets == false)
        {
            output = new GlobalDataModel();
        }
        else
        {
            output = await js.StorageGetItemAsync<GlobalDataModel>(GlobalDataModel.LocalStorageKey);
        }
        GlobalDataModel.DataContext = output;
    }
    public static async Task SaveGlobalDataAsync(this IJSRuntime js)
    {
        if (GlobalDataModel.DataContext == null)
        {
            throw new CustomBasicException("There is no global data.  Should have called the LoadGlobalDataAsync then populated it first");
        }
        if (string.IsNullOrWhiteSpace(GlobalDataModel.DataContext.NickName))
        {
            throw new CustomBasicException("Should have populated the nick name first");
        }
        await js.StorageSetItemAsync(GlobalDataModel.LocalStorageKey, GlobalDataModel.DataContext);
    }
    public static async Task SaveLatestGameAsync(this IJSRuntime js, string gameName, IToast toast)
    {
        if (gameName == "")
        {
            toast.ShowUserErrorToast("Seems to be an error because there was no game name.  If that is not correct. rethink");
        }
        await js.StorageSetStringAsync("latestgame", gameName);
    }
    public static async Task<string> GetLatestGameAsync(this IJSRuntime js)
    {
        bool rets = await js.ContainsKeyAsync("latestgame");
        if (rets == false)
        {
            return "";
        }
        return await js.StorageGetStringAsync("latestgame");
    }
    private static string HostKey => "HostNewGame";
    private static string ClientKey => "ClientNewGame";
    public static async Task<RawGameHost?> GetHostNewGameAsync(this IJSRuntime js)
    {
        if (await js.ContainsKeyAsync(HostKey) == false)
        {
            return null;
        }
        return await js.StorageGetItemAsync<RawGameHost>(HostKey); //well see if this works.
    }
    public static async Task<RawGameClient?> GetClientNewGameAsync(this IJSRuntime js)
    {
        if (await js.ContainsKeyAsync(ClientKey) == false)
        {
            return null;
        }
        return await js.StorageGetItemAsync<RawGameClient>(ClientKey); //well see if this works (?)
    }
    public static async Task DeleteNewGameDataAsync(this IJSRuntime js)
    {
        await js.StorageRemoveItemAsync(HostKey);
        await js.StorageRemoveItemAsync(ClientKey); //delete both.
    }
    public static async Task SaveHostNewGameAsync(this IJSRuntime js, RawGameHost game)
    {
        string content = await js1.SerializeObjectAsync(game); //if this still requires custom serialization, then figure out how to make it for that.
        await js.StorageSetStringAsync(HostKey, content);
    }
    public static async Task SaveClientNewGameAsync(this IJSRuntime js, RawGameClient game)
    {
        string content = await js1.SerializeObjectAsync(game);
        await js.StorageSetStringAsync(ClientKey, content);
    }
}