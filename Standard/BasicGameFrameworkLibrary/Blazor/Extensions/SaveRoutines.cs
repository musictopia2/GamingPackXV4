namespace BasicGameFrameworkLibrary.Blazor.Extensions;
public static class SaveRoutines
{
    public static async Task UpdateLocalStorageAsync(this IJSRuntime js, string key, string value)
    {
        BasicList<string> saveList = StartupClasses.GlobalStartUp.KeysToSave;
        BasicList<string> keyList = await js.GetKeyListAsync();
        await keyList.ForEachAsync(async key =>
        {
            if (saveList.Contains(key) == false)
            {
                await js.StorageRemoveItemAsync(key);
            }
        });
        await js.StorageSetStringAsync(key, value);
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