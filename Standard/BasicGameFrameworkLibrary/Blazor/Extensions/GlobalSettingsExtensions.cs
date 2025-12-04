namespace BasicGameFrameworkLibrary.Blazor.Extensions;
public static class GlobalSettingsExtensions
{
    private static string HostKey => "HostNewGame";
    private static string ClientKey => "ClientNewGame";

    extension(IJSRuntime js)
    {
        public async Task LoadGlobalDataAsync()
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
        public async Task SaveGlobalDataAsync()
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
        public async Task SaveLatestGameAsync(string gameName, IToast toast)
        {
            if (gameName == "")
            {
                toast.ShowUserErrorToast("Seems to be an error because there was no game name.  If that is not correct. rethink");
            }
            await js.StorageSetStringAsync("latestgame", gameName);
        }
        public async Task<string> GetLatestGameAsync()
        {
            bool rets = await js.ContainsKeyAsync("latestgame");
            if (rets == false)
            {
                return "";
            }
            return await js.StorageGetStringAsync("latestgame");
        }
        public async Task<RawGameHost?> GetHostNewGameAsync()
        {
            if (await js.ContainsKeyAsync(HostKey) == false)
            {
                return null;
            }
            return await js.StorageGetItemAsync<RawGameHost>(HostKey); //well see if this works.
        }
        public async Task<RawGameClient?> GetClientNewGameAsync()
        {
            if (await js.ContainsKeyAsync(ClientKey) == false)
            {
                return null;
            }
            return await js.StorageGetItemAsync<RawGameClient>(ClientKey); //well see if this works (?)
        }
        public async Task DeleteNewGameDataAsync()
        {
            await js.StorageRemoveItemAsync(HostKey);
            await js.StorageRemoveItemAsync(ClientKey); //delete both.
        }
        public async Task SaveHostNewGameAsync(RawGameHost game)
        {
            string content = await js1.SerializeObjectAsync(game); //if this still requires custom serialization, then figure out how to make it for that.
            await js.StorageSetStringAsync(HostKey, content);
        }
        public async Task SaveClientNewGameAsync(RawGameClient game)
        {
            string content = await js1.SerializeObjectAsync(game);
            await js.StorageSetStringAsync(ClientKey, content);
        }
    }

}