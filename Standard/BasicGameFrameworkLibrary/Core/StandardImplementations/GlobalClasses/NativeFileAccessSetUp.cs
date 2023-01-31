using static CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileFunctions;
using static CommonBasicLibraries.BasicDataSettingsAndProcesses.ApplicationPath;
using ss1 = System.IO.Path;
namespace BasicGameFrameworkLibrary.Core.StandardImplementations.GlobalClasses;
internal static class NativeFileAccessSetUp
{
    public static string GetParentDirectory()
    {
        string tempPath;
        if (OS == EnumOS.WindowsDT || OS == EnumOS.WindowsMaui) //even windows maui should be able to access the windows stuff.
        {
            tempPath = GetApplicationPath();
        }
        else if (OS == EnumOS.Android) //we don't have xamarin forms.  but we know if its android so some code can run for cases where its android.
        {
            tempPath = GetWriteLocationForExternalOnAndroid();
            tempPath = ss1.Combine(tempPath, "GPXV2");
        }
        else
        {
            throw new CustomBasicException("Only android and windows desktop are supported for native file access implementations");
        }
        tempPath = ss1.Combine(tempPath, "json");
        if (DirectoryExists(tempPath) == false)
        {
            CreateFolder(tempPath);
        }
        return tempPath;
    }
}