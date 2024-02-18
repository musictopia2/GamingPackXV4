namespace GameLoaderBlazorLibrary;
public partial class BasicLoaderPage : IDisposable
{
    [Inject]
    public ILoaderVM? DataContext { get; set; }
    [Inject]
    public IJSRuntime? JS { get; set; }
    private bool _loadedOnce;
    private bool _showSettings;
    private string _previousGame = "";
    private async Task RefreshAsync()
    {
        await JS!.Update(); //i think.
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
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
    private bool _disposedValue;

    protected override void OnInitialized()
    {
        //no longer a need to change latest game.  because it usually refreshes anyways.
        LoaderGlobalClass.BackToMainDelegate = BackToMain;
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
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                DataContext!.StateChanged = null;
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}