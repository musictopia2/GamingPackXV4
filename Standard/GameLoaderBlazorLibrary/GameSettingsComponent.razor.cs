namespace GameLoaderBlazorLibrary;
public partial class GameSettingsComponent
{
    [Parameter]
    public EventCallback CloseSettings { get; set; }
    [Inject]
    IJSRuntime? JS { get; set; } //this should have its own js.
    [Inject]
    private IToast? Toast { get; set; }
    private static string GetRows => gg1.RepeatAuto(7); //for now, 6 instead of 7 (?)
    private static string GetColumns => gg1.RepeatMaximum(2); //may eventually do a special grid control that specialize in adding in data and it would just work.
    private bool _beginaccept;
    protected override void OnInitialized()
    {
        _beginaccept = GlobalDataModel.NickNameAcceptable();
        base.OnInitialized();
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && GlobalDataModel.NickNameAcceptable() == false)
        {
            await _nickElement.FocusAsync();
            return;
        }
        if (_useCustom)
        {
            await _customElement.FocusAsync();
            _useCustom = false;
        }
    }
    private bool _useCustom;
    private void UpdateAzureServerOptions(EnumAzureMode mode)
    {
        GlobalDataModel.DataContext!.AzureMode = mode;
        if (mode == EnumAzureMode.CustomAzure)
        {
            _useCustom = true;
        }
        GlobalDataModel.DataContext.ServerMode = EnumServerMode.AzureHosting; //in this case, do azure hosting.
        BasicGameFrameworkLibrary.Core.MiscProcesses.GlobalVariables.DoUseHome = false;
    }
    private static void UpdateMiscServerOptions(EnumServerMode mode)
    {
        GlobalDataModel.DataContext!.ServerMode = mode;
        //will not update the azure mode though.
        if (mode == EnumServerMode.HomeHosting)
        {
            BasicGameFrameworkLibrary.Core.MiscProcesses.GlobalVariables.DoUseHome = true;
        }
        else
        {
            BasicGameFrameworkLibrary.Core.MiscProcesses.GlobalVariables.DoUseHome = false;
        }
    }
    private async Task SaveChangesAsync()
    {
        if (GlobalDataModel.DataContext == null || JS == null)
        {
            return;
        }
        if (GlobalDataModel.NickNameAcceptable() == false)
        {
            Toast!.ShowUserErrorToast("Needs to enter nick name at least");
            await _nickElement.FocusAsync();
            return;
        }
        if (LoaderGlobalClass.SaveSettingsAsync is null)
        {
            throw new CustomBasicException("Nobody is handling the saving of the settings");
        }
        await LoaderGlobalClass.SaveSettingsAsync.Invoke(JS);
        Toast!.ShowSuccessToast($"Saved Changes On {DateTime.Now}");
        await CloseSettings.InvokeAsync();
    }
    private async Task CancelAsync()
    {
        await CloseSettings.InvokeAsync();
    }
}