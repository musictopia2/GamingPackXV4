namespace BasicGameFrameworkLibrary.Blazor.Extensions;
public static class SaveRoutines
{
    extension (IJSRuntime js)
    {
        public async Task UpdatePrivateGameAsync<T>(string id, string value)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new CustomBasicException("Cannot be blank for gameid");
            }
            await js.StorageSetStringAsync(id, value);
        }
        public async Task<string> GetPrivateGameAsync(string id)
        {
            var item = await js.StorageGetStringAsync(id);
            if (item is null)
            {
                return "";
            }
            return item;
        }
        public async Task UpdateLocalStorageAsync(string key, string value)
        {
            await js.ClearExceptForCurrentGameAsync(key);
            await js.StorageSetStringAsync(key, value); //the private autoresume can just use this one.
        }
        public async Task ClearExceptForCurrentGameAsync(string key)
        {
            BasicList<string> saveList = GlobalStartUp.KeysToSave;
            BasicList<string> keyList = await js.GetKeyListAsync();
            await keyList.ForEachAsync(async item =>
            {
                if (saveList.Any(x => x.Contains(item)) == false && item != key) //does not make sense to delete and add again.
                {
                    await js.StorageRemoveItemAsync(item);
                }
            });
        }
        public async Task DeletePrivateGameAsync(string key)
        {
            BasicList<string> keyList = await js.GetKeyListAsync();
            await keyList.ForEachAsync(async item =>
            {
                if (item.Contains(key))
                {
                    await js.StorageRemoveItemAsync(key);
                }
            });
        }
        private async Task<BasicList<string>> GetKeyListAsync()
        {
            var length = await js.GetLengthAsync();
            BasicList<string> output = new();
            for (int i = 0; i < length; i++)
            {
                int j = i;
                output.Add(await js.KeyAsync(j));

            }
            return output;
        }
        private async Task<string> KeyAsync(int index)
        {
            string output = await js.InvokeAsync<string>("localStorage.key", index);
            return output;
        }
        private async Task<int> GetLengthAsync()
        {
            int output = await js.InvokeAsync<int>("eval", "localStorage.length");
            return output;
        }
    }   
}