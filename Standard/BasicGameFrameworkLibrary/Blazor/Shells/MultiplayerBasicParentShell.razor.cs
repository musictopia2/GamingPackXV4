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
    protected override void OnInitialized()
    {
        BasicData = Resolver!.Resolve<BasicData>();
        BasicData.DoFullScreen = async () =>
        {
            if (NeedsFullScreen() == false)
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
        IStartUp starts = Resolver!.Resolve<IStartUp>();
        starts.StartVariables(BasicData);
        if (BasicData.NickName != "")
        {
            _hadNickName = true;
            _loading = false;
        }
        base.OnInitialized();
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
        return OS == EnumOS.Wasm &&
            (BasicData!.GamePackageMode == EnumGamePackageMode.Debug ||
             LoaderGlobalClass.ShowFullScreenButton);
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


    private bool CanChooseAnotherGame =>
       BasicData!.MultiPlayer &&
       BasicData.Client == false &&
       LoaderGlobalClass.BackToMainDelegate is not null &&
       Resolver!.Resolve<IGameNetwork>().HasServer;

    private async Task ChooseAnotherGameAsync()
    {
        if (CanChooseAnotherGame == false)
        {
            return;
        }
        int humanCount =
            BasicData!.GetOpeningHumanPlayerCount?.Invoke()
            ?? BasicData.GetHumanPlayerCount?.Invoke()
            ?? 0;

        if (humanCount == 0)
        {
            throw new CustomBasicException(
                "The human player roster is not available.");
        }

        LoaderGlobalClass.NextGameHumanCount = humanCount;
        IGameNetwork network = Resolver!.Resolve<IGameNetwork>();

        await network.PrepareForNextGameAsync();
        LoaderGlobalClass.ChoosingNextGameAsHost = true;
        network.IsEnabled = false;
        network.ClearMessages();

        await LoaderGlobalClass.BackToMainDelegate!.Invoke();
    }

    private bool _reloading;

    private bool CanReloadGame =>
        GameData!.CanAutoSave &&
        BasicData!.GameDataLoading == false &&
        TestData!.SaveOption != EnumTestSaveCategory.NoSave &&
        LoaderGlobalClass.ReloadGameAsync is not null &&
        LoaderGlobalClass.PendingReload is null &&
        _reloading == false;

    private async Task ReloadGameAsync()
    {
        if (CanReloadGame == false)
        {
            return;
        }

        _reloading = true;

        try
        {
            bool isLocalGame = BasicData!.MultiPlayer == false;

            if (isLocalGame || BasicData.Client == false)
            {
                IMultiplayerSaveState state =
                    Resolver!.Resolve<IMultiplayerSaveState>();

                EnumRestoreCategory restore = isLocalGame
                    ? await state.SinglePlayerRestoreCategoryAsync()
                    : await state.MultiplayerRestoreCategoryAsync();

                if (restore == EnumRestoreCategory.NoRestore)
                {
                    Toast!.ShowWarningToast(
                        "No saved game is available to reload.");
                    return;
                }
            }

            await LoaderGlobalClass.ReloadGameAsync!.Invoke();
        }
        finally
        {
            _reloading = false;
        }
    }

}