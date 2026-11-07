
namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public static class GamePackageModeContext
{
   
    private static EnumGamePackageMode? _mode;

    internal static EnumGamePackageMode GetGamePackageMode => _mode is null ? throw new CustomBasicException("No mode set") : _mode.Value;

    internal static void SetMode(ISetGamePackageMode mode)
    {
        _mode = mode.Mode;
    }
}