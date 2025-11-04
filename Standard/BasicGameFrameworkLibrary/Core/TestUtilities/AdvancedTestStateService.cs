using jj1 = CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
namespace BasicGameFrameworkLibrary.Core.TestUtilities;
public class AdvancedTestStateService(TestOptions tests, IExit exit)
{
    // Whether to show the advanced button
    public bool CanShowAdvancedButton { get; private set; }
    public Action? StateChanged { get; set; }
    public void EnableAdvancedOptions()
    {
        CanShowAdvancedButton = true;
        StateChanged?.Invoke();
    }
    public async Task CommitAndExitAsync()
    {
        string path = TestOptions.GetTestPath();
        tests.AdvancedTestOptions = false;
        tests.SaveOption = EnumTestSaveCategory.RestoreOnly;
        await jj1.SaveObjectAsync(path, tests);
        exit.ExitApp();
    }
}