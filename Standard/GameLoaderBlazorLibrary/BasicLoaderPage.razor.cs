namespace GameLoaderBlazorLibrary;
public partial class BasicLoaderPage : IDisposable
{
    [Inject]
    public ILoaderVM? DataContext { get; set; }
    [Inject]
    public IJSRuntime? JS { get; set; }
    private bool _loadedOnce;
    private bool _showSettings;
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
            StateHasChanged();
        }
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
        if (aa.Resolver!.RegistrationExist<IGameNetwork>())
        {
            IGameNetwork nets = aa.Resolver!.Resolve<IGameNetwork>();
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
        LoaderGlobalClass.BackToMainDelegate = BackToMain;
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