namespace GameLoaderBlazorLibrary;
public partial class BasicLoaderPage : IDisposable, IAsyncDisposable
{
    [Inject]
    public ILoaderVM? DataContext { get; set; }
    [Inject]
    public IJSRuntime? JS { get; set; }
    private bool _loadedOnce;
    private bool _showSettings;
    private string _previousGame = "";
    private ServiceWorkerInterop? _worker;
    private bool _updateAvailable = false;
    private async Task RefreshAsync()
    {
        await JS!.RefreshBrowser();
    }
    private async Task<string> GetAutomatedGameToLoadAsync()
    {
        //if blank, then means nothing.
        RawGameClient? client = await JS!.GetClientNewGameAsync();
        if (client is not null)
        {
            return client.GameName;
        }
        RawGameHost? host = await JS!.GetHostNewGameAsync();
        if (host is not null)
        {
            return host.GameName;
        }
        return "";
    }
    private async Task OnUpdateAvailableAsync()
    {
        _updateAvailable = true;
        // Show a toast or any UI to notify user
        //maybe no need to show toast this time.
        //Toast?.ShowInfoToast("A new update is available. Please refresh to get the latest version.");
        // State has changed because _updateAvailable changed
        await InvokeAsync(StateHasChanged);
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && bb1.OS == bb1.EnumOS.Wasm && _worker != null)
        {
            await _worker.RegisterAsync(OnUpdateAvailableAsync);
        }
        if (GlobalClass.Multiplayer == false)
        {
            return;
        }
        if (firstRender)
        {
            _loadedOnce = true;
            if (LoaderGlobalClass.LoadSettingsAsync is null)
            {
                throw new CustomBasicException("Nobody is handling the loading of the settings");
            }
            await LoaderGlobalClass.LoadSettingsAsync.Invoke(JS!);
            if (GlobalDataModel.NickNameAcceptable() == false)
            {
                _showSettings = true;
            }
            if (GlobalDataModel.DataContext!.ServerMode == EnumServerMode.HomeHosting)
            {
                BasicGameFrameworkLibrary.Core.MiscProcesses.GlobalVariables.DoUseHome = true;
            }
            else
            {
                BasicGameFrameworkLibrary.Core.MiscProcesses.GlobalVariables.DoUseHome = false;
            }
            if (_showSettings == false)
            {
                _previousGame = await GetAutomatedGameToLoadAsync();
                if (_previousGame != "")
                {
                    OpenPreviousGame();
                    StateHasChanged(); //i think.
                }
            }
            _previousGame = await JS!.GetLatestGameAsync();
            StateHasChanged();
        }
    }
    private void OpenPreviousGame()
    {
        //if you cannot find the previous game, then not found.
        DataContext!.ChoseGame(_previousGame);
        //DataContext!.GameName = _previousGame; //hopefully this simple.
    }
    private bool CanShowGameList()
    {
        if (_showSettings)
        {
            return false; //because the settings are shown.
        }
        if (GlobalClass.Multiplayer == false)
        {
            return true;
        }
        if (GlobalDataModel.DataContext == null)
        {
            return false;
        }
        return string.IsNullOrWhiteSpace(GlobalDataModel.DataContext.NickName) == false;
    }
    private static bool CanStartLoading()
    {
        if (LoaderViewModel.IsSinglePlayerOnly)
        {
            return true;
        }
        if (bb1.OS == bb1.EnumOS.Wasm)
        {
            return true;
        }
        return false; //because if its not web assembly, then only single player games can now do loaders for quite a while.
    }
    private static bool CanRefreshManually()
    {
        if (LoaderViewModel.IsSinglePlayerOnly)
        {
            return false; //because those games never had issues to begin with.
        }
        if (bb1.OS == bb1.EnumOS.Wasm)
        {
            return true;
        }
        return false;
    }
    private async void BackToMain()
    {
        if (CanRefreshManually())
        {
            await JS!.RefreshBrowser();
            return;
        } //if its refreshing, then no need to communicate anymore because old would have been disposed (i think).
        if (aa1.Resolver!.RegistrationExist<IGameNetwork>())
        {
            IGameNetwork nets = aa1.Resolver!.Resolve<IGameNetwork>();
            await nets.BackToMainAsync();
        }
        DataContext!.GameName = "";
        StateHasChanged();
    }
    private void ClosedSettings()
    {
        _showSettings = false;
    }
    private void OpenSettings()
    {
        _showSettings = true;
    }

    protected override void OnInitialized()
    {
        //no longer a need to change latest game.  because it usually refreshes anyways.
        LoaderGlobalClass.BackToMainDelegate = BackToMain;
        if (bb1.OS == bb1.EnumOS.Wasm)
        {
            _worker = new(JS!);
        }
        //LoaderGlobalClass.ChangeLatestGame = (string game) =>
        //{
        //    _previousGame = game;
        //};
        DataContext!.StateChanged = () => InvokeAsync(StateHasChanged);
        
        if (GlobalClass.Multiplayer == false)
        {
            _loadedOnce = true; //because not important since not using the settings.
        }
        base.OnInitialized();
    }
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // Synchronous cleanup
            DataContext!.StateChanged = null;
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);

        // Ensure async resources are disposed as well
        DisposeAsync().AsTask().GetAwaiter().GetResult();

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_worker is not null)
        {
            await _worker.DisposeAsync();
        }

        // Call Dispose to run synchronous cleanup
        Dispose(disposing: false);

        GC.SuppressFinalize(this);
    }
}