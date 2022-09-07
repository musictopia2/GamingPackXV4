using BasicGameFrameworkLibrary.Core.StandardImplementations.GlobalClasses; //not common enough.
using static CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileFunctions;
using fs = CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using ss = System.IO.Path;
namespace BasicGameFrameworkLibrary.Core.StandardImplementations.AutoResumeNativeFileAccessClasses;
public class MultiPlayerReleaseNativeFileAccessAutoResume : IMultiplayerSaveState
{
    private readonly IGameInfo _game;
    private readonly BasicData _data;
    private readonly TestOptions _test;
    private readonly string _localPath;
    private readonly string _multiPath;
    public MultiPlayerReleaseNativeFileAccessAutoResume(IGameInfo game, BasicData data, TestOptions test)
    {
        string tempPath = NativeFileAccessSetUp.GetParentDirectory();
        _game = game;
        _data = data;
        _test = test;
        _localPath = ss.Combine(tempPath, $"{game.GameName} SingleRelease.json");
        _multiPath = ss.Combine(tempPath, $"{game.GameName} MultiplayerRelease.json");
    }
    async Task IMultiplayerSaveState.DeleteGameAsync()
    {
        if (CanChange() == false)
        {
            return;
        }
        if (_data.MultiPlayer == false)
        {
            await DeleteFileAsync(_localPath);
        }
        else
        {
            await DeleteFileAsync(_multiPath);
        }
    }
    async Task IMultiplayerSaveState.DeleteMultiplayerGameAsync()
    {
        await DeleteFileAsync(_multiPath);
    }
    async Task<EnumRestoreCategory> IMultiplayerSaveState.MultiplayerRestoreCategoryAsync()
    {
        await Task.Delay(0);
        if (_test.SaveOption == EnumTestSaveCategory.NoSave)
        {
            return EnumRestoreCategory.NoRestore;
        }
        bool rets = FileExists(_multiPath);
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
        await Task.Delay(0);
        if (_test.SaveOption == EnumTestSaveCategory.NoSave)
        {
            return EnumRestoreCategory.NoRestore;
        }
        bool rets = FileExists(_localPath);
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
        string pathUsed;
        if (_data.MultiPlayer)
        {
            pathUsed = _multiPath;
        }
        else
        {
            pathUsed = _localPath;
        }
        await fs.SaveObjectAsync(pathUsed, thisState);
    }
    async Task<string> IMultiplayerSaveState.TempMultiSavedAsync()
    {
        if (_game.CanAutoSave == false || _test.SaveOption == EnumTestSaveCategory.NoSave)
        {
            return "";
        }
        if (FileExists(_multiPath) == false)
        {
            return "";
        }
        return await AllTextAsync(_multiPath); //i think
    }
    async Task<string> IMultiplayerSaveState.SavedDataAsync<T>()
    {
        if (_game.CanAutoSave == false || _test.SaveOption == EnumTestSaveCategory.NoSave)
        {
            return "";
        }
        string pathUsed ;
        if (_data.MultiPlayer == false && FileExists(_localPath) == false)
        {
            return "";
        }
        if (_data.MultiPlayer && FileExists(_multiPath) == false)
        {
            return "";
        }
        if (_data.MultiPlayer)
        {
            pathUsed = _multiPath;
        }
        else
        {
            pathUsed = _localPath;
        }
        string output = await AllTextAsync(pathUsed);
        return output;
    }
}