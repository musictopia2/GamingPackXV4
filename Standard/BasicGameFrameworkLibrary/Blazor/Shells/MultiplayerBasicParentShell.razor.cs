namespace BasicGameFrameworkLibrary.Blazor.Shells;
public partial class MultiplayerBasicParentShell
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [CascadingParameter]
    private MediaQueryListComponent? Media { get; set; }
    public BasicData? BasicData { get; set; } //maybe this should be cascaded.
    public IGameInfo? GameData { get; set; } //i don't think this one needs to
    public TestOptions? TestData { get; set; }
    //good news is worked in desktop mode.
    //however, very iffy because i would like the possibility of not even requiring the js.
    //could be okay (for now. not sure).
    [Inject]
    private IJSRuntime? JS { get; set; } //now needs this because will attempt to do into local storage even on desktop mode (not sure if this will work or not).
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
        BasicData = Resolver!.Resolve<BasicData>();
        BasicData.DoFullScreen = async () =>
        {
            if (BasicData.GamePackageMode is not EnumGamePackageMode.Debug)
            {
                return;
            }
            await JS!.OpenFullScreen();
            _fullScreen = true;
            StateHasChanged(); //i think.
        };
        TestData = Resolver!.Resolve<TestOptions>();
        GameData = Resolver!.Resolve<IGameInfo>();
        CommandContainer command = Resolver!.Resolve<CommandContainer>();
        command.ParentAction = StateHasChanged;
        IStartUp starts = Resolver!.Resolve<IStartUp>();
        starts.StartVariables(BasicData); //eventually would do something else to figure out who it is.
        if (BasicData.NickName != "")
        {
            _hadNickName = true; //because it got it somehow even if from native or other external process.
            _loading = false; //try this.
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
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (BasicData == null || JS == null || _hadNickName)
        {
            return;
        }
        if (firstRender && BasicData.NickName == "")
        {
            string item = await JS.StorageGetStringAsync("nickname"); //maybe here but different behavior with wasm
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
            StateHasChanged(); //try this too.
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