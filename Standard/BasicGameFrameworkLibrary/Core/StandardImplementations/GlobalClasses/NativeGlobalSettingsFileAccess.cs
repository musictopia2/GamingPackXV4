using BasicGameFrameworkLibrary.Core.StandardImplementations.Settings;
using ff1 = CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileFunctions;
using jj1 = CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using ss1 = System.IO.Path;
namespace BasicGameFrameworkLibrary.Core.StandardImplementations.GlobalClasses;
public static class NativeGlobalSettingsFileAccess
{
    private static string GetGlobalSettingsPath()
    {
        string output = NativeFileAccessSetUp.GetParentDirectory();
        output = ss1.Combine(output, "settings.json");
        return output;
    }
    public static async Task LoadSettingsAsync()
    {
        GlobalDataModel output;
        string tempPath = GetGlobalSettingsPath();
        if (ff1.FileExists(tempPath) == false)
        {
            output = new();
        }
        else
        {
            output = await jj1.RetrieveSavedObjectAsync<GlobalDataModel>(tempPath);
        }
        GlobalDataModel.DataContext = output;
    }
    public static async Task SaveSettingsAsync()
    {
        if (GlobalDataModel.DataContext is null)
        {
            throw new CustomBasicException("Must have the data information filled out");
        }
        string tempPath = GetGlobalSettingsPath();
        await jj1.SaveObjectAsync(tempPath, GlobalDataModel.DataContext);
    }
}