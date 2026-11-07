namespace GameLoaderBlazorLibrary;
public partial class GameSettingsComponent
{
    [Parameter]
    public EventCallback CloseSettings { get; set; }

    [Inject]
    private IJSRuntime? JS { get; set; }

    [Inject]
    private IToast? Toast { get; set; }

    private static string GetRows => gg1.RepeatAuto(8);
    private static string GetColumns => gg1.RepeatMaximum(2);
    private bool _showPlayerSettings = false;

    private void CancelPlayerSettings()
    {
        _showPlayerSettings = false;
    }

    


    private void ShowPlayerSettings()
    {
        _showPlayerSettings = true;
    }

    private bool _beginaccept;

    protected override void OnInitialized()
    {
        _beginaccept = GlobalDataModel.NickNameAcceptable();
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender
            && GlobalDataModel.DataContext != null
            && GlobalDataModel.NickNameAcceptable() == false)
        {
            await _nickElement.FocusAsync();
        }
    }

    private static void UpdateServerOptions(EnumServerMode mode)
    {
        GlobalDataModel.DataContext!.ServerMode = mode;

        HomeNetworkConfiguration.DoUseHome =
            mode == EnumServerMode.HomeHosting;
    }

    private void UpdatePlayerOptions(EnumPlayerMode mode)
    {
        if (mode == EnumPlayerMode.None)
        {
            return;
        }
        GlobalDataModel.DataContext!.PlayerMode = mode;
        _showPlayerSettings = false;
    }

    private async Task SaveChangesAsync()
    {
        var settings = GlobalDataModel.DataContext;

        if (settings == null || JS == null)
        {
            return;
        }
        if (settings.PlayerMode == EnumPlayerMode.None)
        {
            Toast!.ShowUserErrorToast("Must choose player mode option");
            //nothing to focus on (for now, hopefully okay).
            //when going to the screen, don't show old options.
            return;
        }
        if (GlobalDataModel.NickNameAcceptable() == false)
        {
            Toast!.ShowUserErrorToast(
                "Enter a nickname using only letters a–z.");
            await _nickElement.FocusAsync();
            return;
        }

        if (LoaderGlobalClass.SaveSettingsAsync is null)
        {
            throw new CustomBasicException(
                "Nobody is handling the saving of the settings");
        }

        settings.CustomHostedServerUrl =
            settings.CustomHostedServerUrl?.Trim() ?? "";

        await LoaderGlobalClass.SaveSettingsAsync.Invoke(JS);

        Toast!.ShowSuccessToast($"Saved Changes On {DateTime.Now}");
        await CloseSettings.InvokeAsync();
    }

    private async Task CancelAsync()
    {
        await CloseSettings.InvokeAsync();
    }
}