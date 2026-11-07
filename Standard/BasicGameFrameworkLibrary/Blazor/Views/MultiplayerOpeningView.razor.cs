
namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class MultiplayerOpeningView<P> : IHandle<OpenMultiplayerOpeningEventModel<P>>,
    IHandle<CloseMultiplayerOpeningEventModel<P>>, IDisposable
     where P : class, IPlayerItem, new()
{

    private MultiplayerOpeningViewModel<P>? DataContext { get; set; }
    private partial void Subscribe();
    private partial void Unsubscribe();
    private readonly IEventAggregator _aggregator;
    public MultiplayerOpeningView()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
    }
    void IDisposable.Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
    [CascadingParameter]
    public IGameInfo? GameData { get; set; }
    private bool _canHuman;
    private bool _canComputer;
    private readonly BasicList<LabelGridModel> _labels = [];
    private RawGameHost? _hostNewGameInfo;
    private RawGameClient? _clientNewGameInfo;
    private bool _realLoad;
    protected override void OnInitialized()
    {
        _aggregator.Publish(new AskMultiplayerOpeningEventModel<P>());
        _labels.Clear();
        _labels.AddLabel("Players Connected", nameof(IMultiplayerOpeningDisplayVM.ClientsConnected))
            .AddLabel("Previous Players", nameof(IMultiplayerOpeningDisplayVM.PreviousNonComputerNetworkedPlayers));
        base.OnInitialized();
    }
    private void Refresh()
    {
        if (DataContext == null || GameData == null)
        {
            return;
        }
        _canHuman = OpenPlayersHelper.CanHuman(GameData);
        _canComputer = OpenPlayersHelper.CanComputer(GameData);
    }
    protected override void OnParametersSet()
    {
        Refresh();
        
        base.OnParametersSet();
    }
    private static EnumPlayerMode GetPlayerMode()
    {
        if (GlobalDataModel.DataContext is null)
        {
            return EnumPlayerMode.AllOptions; //this means for test game package, all options are available.
        }
        return GlobalDataModel.DataContext.PlayerMode;
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_realLoad == false &&
            DataContext?.IsOpeningReady == true &&
            GameData is not null)
        {
            _hostNewGameInfo = await JS!.GetHostNewGameAsync();
            _clientNewGameInfo = await JS!.GetClientNewGameAsync();
            if (_hostNewGameInfo is not null && _clientNewGameInfo is not null)
            {
                throw new CustomBasicException("Cannot have both client and host.  Something is wrong now");
            }
            await JS!.DeleteNewGameDataAsync(); //go ahead and delete.  so if i start over again, has to do over again.
            _realLoad = true;
            ReloadGameRequest? reload = LoaderGlobalClass.PendingReload;

            if (reload is not null)
            {
                LoaderGlobalClass.PendingReload = null;

                if (reload.GameName != GameData.GameName ||
                    GameData.CanAutoSave == false)
                {
                    BasicBlazorLibrary.Helpers.BlazorUIHelpers.Toast!
                        .ShowWarningToast("This game cannot be reloaded.");

                    StateHasChanged();
                    return;
                }
                BasicData basic = Resolver!.Resolve<BasicData>();

                if (reload.IsLocalGame)
                {
                    if (DataContext!.CanResumeSinglePlayer == false)
                    {
                        BasicBlazorLibrary.Helpers.BlazorUIHelpers.Toast!
                            .ShowWarningToast(
                                "No saved local game is available to reload.");

                        StateHasChanged();
                        return;
                    }

                    basic.ReloadInProgress = true;
                    await DataContext.ResumeSinglePlayerAsync();
                    return;
                }

                if (reload.IsClient)
                {
                    basic.ReloadInProgress = true;
                    await DataContext!.ConnectAsync();
                    return;
                }

                if (DataContext!.CanRejoinMultiplayerGame() == false)
                {
                    BasicBlazorLibrary.Helpers.BlazorUIHelpers.Toast!
                        .ShowWarningToast(
                            "No saved multiplayer game is available to reload.");

                    StateHasChanged();
                    return;
                }
                basic.ReloadInProgress = true;
                await DataContext.RejoinMultiplayerGameAsync();
                return;
            }
            if (_hostNewGameInfo is not null)
            {
                NewGameContainer.NewGameHost = _hostNewGameInfo;
                if (_hostNewGameInfo.Multiplayer == false)
                {
                    await DataContext!.StartAnotherSinglePlayerGameAsync();
                    return; //hopefully does not need to do statehaschanged because its automatically loading another game.
                }
                await DataContext!.HostAsync();
                return;
            }
            if (_clientNewGameInfo is not null)
            {
                await DataContext!.ConnectAsync();
                return;
            }
            if (LoaderGlobalClass.AutoHostGameName == GameData!.GameName)
            {
                LoaderGlobalClass.AutoHostGameName = "";
                await DataContext!.HostAsync();
                return;
            }
            if (GetPlayerMode() == EnumPlayerMode.LetHostChoose)
            {
                await DataContext!.ConnectAsync();
                return;
            }
            StateHasChanged(); //has to reload the state now.
        }
    }

    void IHandle<OpenMultiplayerOpeningEventModel<P>>.Handle(OpenMultiplayerOpeningEventModel<P> message)
    {
        DataContext = message.DataContext;
        InvokeAsync(StateHasChanged);
    }

    void IHandle<CloseMultiplayerOpeningEventModel<P>>.Handle(CloseMultiplayerOpeningEventModel<P> message)
    {
        DataContext = null;
        InvokeAsync(StateHasChanged);
    }
}