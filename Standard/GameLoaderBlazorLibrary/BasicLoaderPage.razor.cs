namespace GameLoaderBlazorLibrary;
public partial class BasicLoaderPage : IAsyncDisposable
{
    [Inject]
    public ILoaderVM? DataContext { get; set; }
    [Inject]
    public IJSRuntime? JS { get; set; }
    [CascadingParameter]
    private MediaQueryListComponent Media { get; set; } = null!;


    private string _nextGameHost = "";
    private string _gameToReload = "";

    private Task ReloadGameAsync()
    {
        return InvokeAsync(async () =>
        {
            if (DataContext!.GameName == "" ||
                LoaderGlobalClass.PendingReload is not null)
            {
                return;
            }

            BasicData data = aa1.Resolver!.Resolve<BasicData>(); //not sure maybe.

            ReloadGameRequest request = new()
            {
                GameName = DataContext.GameName,
                IsClient = data.Client,
                IsLocalGame = data.MultiPlayer == false
            };

            LoaderGlobalClass.PendingReload = request;

            try
            {
                await BackToMainAsync();
            }
            catch
            {
                LoaderGlobalClass.PendingReload = null;
                throw;
            }

            _gameToReload = request.GameName;
            StateHasChanged();
        });
    }


    private Task WaitForNextGameAsync(string hostName)
    {
        return InvokeAsync(async () =>
        {
            _nextGameHost = hostName;
            await BackToMainAsync();
        });
    }

    protected override void OnParametersSet()
    {
        LayoutInfo.LayoutMode = Media.DeviceSize switch
        {
            EnumDeviceSize.SmallPhone or EnumDeviceSize.LargePhone
                or EnumDeviceSize.SmallTablet => EnumGameLayoutMode.Compact,
            EnumDeviceSize.LargeTablet => EnumGameLayoutMode.Tablet,
            EnumDeviceSize.Desktop => EnumGameLayoutMode.Desktop,
            _ => EnumGameLayoutMode.Desktop
        };
        base.OnParametersSet();
    }
    private static string Version => $"Version: {GlobalClass.Version}";

    private bool _showHostDiscovery = false;

    private bool _showCompactCompatibleOnly;
    //should be obvious because less games when its compact only.
    //refreshing puts back to entire list.
    public void ToggleCompactCompatible()
    {
        _showCompactCompatibleOnly = !_showCompactCompatibleOnly;
    }
    public void ShowHostDiscovery()
    {
        _showHostDiscovery = true;
    }
    public void CancelWaiting()
    {
        _showHostDiscovery = false;
        _nextGameHost = "";
    }


    private bool IsGameSupported(string game)
    {
        if (DataContext!.GameList.Contains(game) == false)
        {
            return false;
        }
        if (LoaderGlobalClass.ChoosingNextGameAsHost)
        {
            GameRegistration? registration =
                DataContext!.GetGameRegistration(game);

            if (registration is null ||
                registration.SupportsHumanGroup(
                    LoaderGlobalClass.NextGameHumanCount) == false)
            {
                return false;
            }
        }
        bool requiresCompact =
            LayoutInfo.LayoutMode == EnumGameLayoutMode.Compact
            || _showCompactCompatibleOnly;

        return requiresCompact == false
            || DataContext.SupportsCompactLayout(game);
    }
    private BasicList<string> GetVisibleGames()
    {
        BasicList<string> games = [];
        foreach (string game in DataContext!.GameList)
        {
            if (IsGameSupported(game))
            {
                games.Add(game);
            }
        }
        return games;
    }

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
    private async Task SelectDiscoveredGameAsync(string game)
    {
        _nextGameHost = "";
        _showHostDiscovery = false;
        if (IsGameSupported(game) == false)
        {
            BlazorUIHelpers.Toast!.ShowWarningToast(
                $"The host chose {game}, which is not available on this device.");
            return;
        }
        await SelectGameAsync(game);
    }

    private async Task SelectGameAsync(string game)
    {
        if (IsGameSupported(game) == false)
        {
            return;
        }
        await JS!.SaveLatestGameAsync(game, BlazorUIHelpers.Toast!);
        _previousGame = game;
        LoaderGlobalClass.AutoHostGameName =
            LoaderGlobalClass.ChoosingNextGameAsHost ? game : "";

        LoaderGlobalClass.ChoosingNextGameAsHost = false;
        DataContext!.ChoseGame(game);
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
        if (_gameToReload != "" && DataContext!.GameName == "")
        {
            string game = _gameToReload;
            _gameToReload = "";

            await SelectGameAsync(game);
            return;
        }

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
                HomeNetworkConfiguration.DoUseHome = true;
            }
            else
            {
                HomeNetworkConfiguration.DoUseHome = false;
            }
            if (_showSettings == false)
            {
                _previousGame = await GetAutomatedGameToLoadAsync();
                if (_previousGame != "")
                {
                    await OpenPreviousGameAsync();
                    StateHasChanged(); //i think.
                }
            }
            _previousGame = await JS!.GetLatestGameAsync();
            StateHasChanged();
        }
    }
    private async Task OpenPreviousGameAsync()
    {
        await SelectGameAsync(_previousGame);
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
    private async Task BackToMainAsync()
    {
        if (aa1.Resolver!.RegistrationExist<IGameNetwork>())
        {
            IGameNetwork nets = aa1.Resolver!.Resolve<IGameNetwork>();
            await nets.BackToMainAsync();
        }
        js1.RequireCustomSerialization = false;
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
        LoaderGlobalClass.BackToMainDelegate = BackToMainAsync;
        LoaderGlobalClass.ReloadGameAsync = ReloadGameAsync;
        if (bb1.OS == bb1.EnumOS.Wasm)
        {
            _worker = new(JS!);
        }
        DataContext!.StateChanged = () => InvokeAsync(StateHasChanged);
        if (GlobalClass.Multiplayer == false)
        {
            _loadedOnce = true; //because not important since not using the settings.
        }
        else
        {
            LoaderGlobalClass.WaitForNextGameAsync = WaitForNextGameAsync;
        }
        base.OnInitialized();
    }
    private bool _disposed;
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        LoaderGlobalClass.WaitForNextGameAsync = null;
        LoaderGlobalClass.ReloadGameAsync = null;
        LoaderGlobalClass.PendingReload = null;
        DataContext?.StateChanged = null;

        if (_worker is not null)
        {
            await _worker.DisposeAsync();
            _worker = null;
        }
        GC.SuppressFinalize(this);
    }
}