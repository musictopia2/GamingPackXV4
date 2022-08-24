using static CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileFunctions;
using static CommonBasicLibraries.BasicDataSettingsAndProcesses.ApplicationPath;
//not common enough to put in global for these 3 areas.
namespace BasicGameFrameworkLibrary.Core.StandardImplementations.GlobalClasses;
internal static class NativeFileAccessSetUp
{
    //hopefully this is the only change needed (?)
    public static string GetParentDirectory()
    {
        string tempPath;
        if (OS == EnumOS.WindowsDT)
        {
            tempPath = GetApplicationPath();
        }
        else if (OS == EnumOS.Android) //we don't have xamarin forms.  but we know if its android so some code can run for cases where its android.
        {
            tempPath = GetWriteLocationForExternalOnAndroid();
            tempPath = Path.Combine(tempPath, "GPXV2");
        }
        else
        {
            throw new CustomBasicException("Only android and windows desktop are supported for native file access implementations");
        }
        tempPath = Path.Combine(tempPath, "json"); //only desktop does it.  since xamarin form does something else.
        if (DirectoryExists(tempPath) == false)
        {
            _ = Directory.CreateDirectory(tempPath);
        }
        return tempPath;
    }
}