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
    private async void BackToMain()
    {
        if (aa1.Resolver!.RegistrationExist<IGameNetwork>())
        {
            IGameNetwork nets = aa1.Resolver!.Resolve<IGameNetwork>();
            await nets.BackToMainAsync();
        }
        await JS!.RefreshBrowser();
        //decided to go ahead and refresh the browser.  this would make it where you can go back to the game and would be completely refreshed as intended (since i can't seem to control removing all objects to start fresh again).
        //DataContext!.GameName = "";
        //StateHasChanged();
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
        LoaderGlobalClass.BackToMainDelegate = BackToMain;
        LoaderGlobalClass.ChangeLatestGame = (string game) =>
        {
            _previousGame = game;
        };
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