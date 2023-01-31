using BasicGameFrameworkLibrary.Core.StandardImplementations.GlobalClasses; //not common enough to have everywhere.
using static CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileFunctions;
using ss1 = System.IO.Path;
using fs1 = CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
namespace BasicGameFrameworkLibrary.Core.StandardImplementations.AutoResumeNativeFileAccessClasses;
public class SinglePlayerReleaseNativeFileAccessAutoResume : ISaveSinglePlayerClass
{
    private readonly IGameInfo _thisGame;
    private readonly string _gamePath;
    public SinglePlayerReleaseNativeFileAccessAutoResume(IGameInfo thisGame)
    {
        _thisGame = thisGame;
        string tempPath = NativeFileAccessSetUp.GetParentDirectory();
        _gamePath = ss1.Combine(tempPath, $"{_thisGame.GameName}Release.json");
    }
    Task<bool> ISaveSinglePlayerClass.CanOpenSavedSinglePlayerGameAsync()
    {
        if (_thisGame.CanAutoSave == false)
        {
            return Task.FromResult(false);
        }
        return Task.FromResult(FileExists(_gamePath));
    }
    async Task ISaveSinglePlayerClass.DeleteSinglePlayerGameAsync()
    {
        if (_thisGame.CanAutoSave == false)
        {
            return;
        }
        await DeleteFileAsync(_gamePath);
    }
    async Task<T> ISaveSinglePlayerClass.RetrieveSinglePlayerGameAsync<T>()
    {
        if (_thisGame.CanAutoSave == false)
        {
            throw new CustomBasicException("Should not have autosaved.  Should have first called CanOpenSavedSinglePlayerGameAsync To See");
        }
        T output = await fs1.RetrieveSavedObjectAsync<T>(_gamePath);
        return output;
    }
    async Task ISaveSinglePlayerClass.SaveSimpleSinglePlayerGameAsync<T>(T thisObject)
    {
        if (_thisGame.CanAutoSave == false)
        {
            return;
        }
        if (thisObject is null)
        {
            throw new CustomBasicException("Cannot save null object.  Rethink");
        }
        await fs1.SaveObjectAsync(_gamePath, thisObject);
    }
}