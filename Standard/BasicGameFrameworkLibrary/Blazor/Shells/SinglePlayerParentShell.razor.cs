namespace BasicGameFrameworkLibrary.Blazor.Shells;
public partial class SinglePlayerParentShell
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    public BasicData BasicData { get; set; }
    public IGameInfo GameInfo { get; set; }
    public SinglePlayerParentShell()
    {
        BasicData = Resolver!.Resolve<BasicData>();
        GameInfo = Resolver!.Resolve<IGameInfo>();
    }
    protected override void OnInitialized()
    {
        IStartUp starts = Resolver!.Resolve<IStartUp>();
        starts.StartVariables(BasicData);
        BasicData.ChangeState = ShowChange;
        base.OnInitialized();
    }
    private async void ShowChange()
    {
        await InvokeAsync(StateHasChanged);
    }


    private bool _reloading;

    private bool CanReloadGame =>
        GameInfo.CanAutoSave &&
        BasicData.GameDataLoading == false &&
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
            ISaveSinglePlayerClass saves =
                Resolver!.Resolve<ISaveSinglePlayerClass>();

            bool hasSavedGame =
                await saves.CanOpenSavedSinglePlayerGameAsync();

            if (hasSavedGame == false)
            {
                Resolver!.Resolve<IToast>().ShowWarningToast(
                    "No saved game is available to reload.");
                return;
            }

            await LoaderGlobalClass.ReloadGameAsync!.Invoke();
        }
        finally
        {
            _reloading = false;
        }
    }

}