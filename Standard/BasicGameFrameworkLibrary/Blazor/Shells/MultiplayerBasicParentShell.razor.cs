namespace BasicGameFrameworkLibrary.Blazor.Shells;
public partial class MultiplayerBasicParentShell
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [CascadingParameter]
    private MediaQueryListComponent? Media { get; set; }
    public BasicData? BasicData { get; set; }
    public IGameInfo? GameData { get; set; }
    public TestOptions? TestData { get; set; }
    private AdvancedTestStateService? AdvancedState { get; set; }
    private IToast? Toast { get; set; }
    [Inject]
    private IJSRuntime? JS { get; set; }
    private bool _loading = true;
    private bool _hadNickName;
    private bool _fullScreen;
    private string GetDisplay => _fullScreen == false ? "Open Full Screen" : "Exit Full Screen";
    private string FontSize => Media!.DeviceCategory == EnumDeviceCategory.Phone ? ".8rem;" : "1rem;";
    private bool IsSupported()
    {
        if (Media!.DeviceCategory == EnumDeviceCategory.Phone && GameData!.SmallestSuggestedSize != EnumSmallestSuggested.AnyDevice)
        {
            return false;
        }
        return true; //for now.
    }
    protected override void OnInitialized()
    {
        NewGameDelegates.NewGameHostStep1 = NewGameHostStep1Async;
        BasicData = Resolver!.Resolve<BasicData>();
        BasicData.DoFullScreen = async () =>
        {
            if (BasicData.GamePackageMode is not EnumGamePackageMode.Debug)
            {
                return;
            }
            await JS!.OpenFullScreen();
            _fullScreen = true;
            StateHasChanged();
        };
        TestData = Resolver!.Resolve<TestOptions>();
        GameData = Resolver!.Resolve<IGameInfo>();
        AdvancedState = Resolver!.Resolve<AdvancedTestStateService>();
        AdvancedState.StateChanged = StateHasChanged;
        Toast = Resolver!.Resolve<IToast>();
        CommandContainer command = Resolver!.Resolve<CommandContainer>();
        command.ParentAction = StateHasChanged;
        IStartUp starts = Resolver!.Resolve<IStartUp>();
        starts.StartVariables(BasicData);
        if (BasicData.NickName != "")
        {
            _hadNickName = true;
            _loading = false;
        }
        base.OnInitialized();
    }
    private async Task NewGameHostStep1Async(RawGameHost game)
    {
        await JS!.SaveHostNewGameAsync(game);
        var item = OS;
        if (item == EnumOS.Wasm)
        {
            await JS!.RefreshBrowser(); //this cannot care what happens next.
            return;
        }
        if (item == EnumOS.WindowsDT)
        {
            if (NewGameDelegates.ReloadOnWPF is null)
            {
                Toast!.ShowUserErrorToast("Nobody is reloading on WPF.  Rethink");
                return;
            }
            NewGameDelegates.ReloadOnWPF.Invoke();
            return;
        }
        throw new CustomBasicException("Only wasm and windows desktop is supported");
        //there is a good chance i cannot even support maui anymore.
        //maui cannot even go back to main either.



        //Toast!.ShowInfoToast("Parent Shell Is Starting To Handle New Game");
    }
    private async Task ManuallyOpenCloseFullScreenAsync()
    {
        if (_fullScreen)
        {
            await JS!.ExitFullScreen();
            _fullScreen = false;
        }
        else
        {
            await JS!.OpenFullScreen();
            _fullScreen = true;
        }
    }
    private static Task ProcessNickNameAsync()
    {
        return Task.CompletedTask;
    }
    private bool NeedsFullScreen()
    {
        if (BasicData!.GamePackageMode == EnumGamePackageMode.Debug)
        {
            if (OS == EnumOS.Wasm)
            {
                return true;
            }
            return false;
        }
        return false;
    }
    private bool _loadedGame;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_loadedGame == false && GameData!.GameName != "")
        {
            await JS!.SaveLatestGameAsync(GameData!.GameName, Toast!);
            _loadedGame = true;
        }
        if (BasicData == null || JS == null || _hadNickName)
        {
            return;
        }
        if (firstRender && BasicData.NickName == "")
        {
            string item = await JS.StorageGetStringAsync("nickname");
            if (item is not null)
            {
                item = item.Replace(Constants.QQ, "");
            }
            if (string.IsNullOrWhiteSpace(item) == false)
            {
                BasicData.NickName = item;
                _hadNickName = true;
            }
            _loading = false;
            StateHasChanged();
        }
    }
    protected override void OnParametersSet()
    {
        if (BasicData == null)
        {
            return;
        }
        if (_hadNickName == false && BasicData.NickName != "")
        {
            _loading = true;

        }
        base.OnParametersSet();
    }
    private async void SetNickNameAsync()
    {
        if (BasicData == null || JS == null)
        {
            return;
        }
        if (_hadNickName == false && BasicData.NickName != "")
        {
            await JS.StorageSetStringAsync("nickname", BasicData.NickName);
        }
    }
}