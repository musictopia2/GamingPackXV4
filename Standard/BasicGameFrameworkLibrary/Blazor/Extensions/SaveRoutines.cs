namespace BasicGameFrameworkLibrary.Blazor.Extensions;
public static class SaveRoutines
{
    public static async Task UpdatePrivateGameAsync<T>(this IJSRuntime js, string id, string value)
    {
        await js.StorageSetStringAsync(id, value);
    }
    public static async Task<string> GetPrivateGameAsync(this IJSRuntime js, string id)
    {
        if (js.ContainsKey(id) == false)
        {
            return "";
        }
        return await js.StorageGetStringAsync(id);
    }
    public static async Task UpdateLocalStorageAsync(this IJSRuntime js, string key, string value)
    {
        BasicList<string> saveList = GlobalStartUp.KeysToSave;
        BasicList<string> keyList = await js.GetKeyListAsync();
        await keyList.ForEachAsync(async key =>
        {
            if (saveList.Any(x => x.Contains(key)) == false)
            {
                await js.StorageRemoveItemAsync(key);
            }
        });
        await js.StorageSetStringAsync(key, value); //the private autoresume can just use this one.
    }
    public static async Task DeletePrivateGameAsync(this IJSRuntime js, string key)
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
    private static async Task<BasicList<string>> GetKeyListAsync(this IJSRuntime js)
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
    private static async Task<string> KeyAsync(this IJSRuntime js, int index)
    {
        string output = await js.InvokeAsync<string>("localStorage.key", index);
        return output;
    }
    private static async Task<int> GetLengthAsync(this IJSRuntime js)
    {
        int output = await js.InvokeAsync<int>("eval", "localStorage.length");
        return output;
    }
}