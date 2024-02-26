namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class MultiplayerOpeningView<P>
     where P : class, IPlayerItem, new()
{
    [CascadingParameter]
    public MultiplayerOpeningViewModel<P>? DataContext { get; set; }
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
        _labels.Clear();
        _labels.AddLabel("Players Connected", nameof(IMultiplayerOpeningViewModel.ClientsConnected))
            .AddLabel("Previous Players", nameof(IMultiplayerOpeningViewModel.PreviousNonComputerNetworkedPlayers));
        base.OnInitialized();
    }
    protected override void OnParametersSet()
    {
        if (DataContext == null || GameData == null)
        {
            return;
        }
        _canHuman = OpenPlayersHelper.CanHuman(GameData);
        _canComputer = OpenPlayersHelper.CanComputer(GameData);
        base.OnParametersSet();
    }
    private static EnumPlayerMode GetPlayerMode()
    {
        if (GlobalDataModel.DataContext is null)
        {
            return EnumPlayerMode.Any;
        }
        return GlobalDataModel.DataContext.PlayerMode;
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _hostNewGameInfo = await JS!.GetHostNewGameAsync();
            _clientNewGameInfo = await JS!.GetClientNewGameAsync();
            if (_hostNewGameInfo is not null && _clientNewGameInfo is not null)
            {
                throw new CustomBasicException("Cannot have both client and host.  Something is wrong now");
            }
            await JS!.DeleteNewGameDataAsync(); //go ahead and delete.  so if i start over again, has to do over again.
            _realLoad = true;
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
                await DataContext!.ConnectAsync(); //try this way (?)
                return;
            }
            StateHasChanged(); //has to reload the state now.
        }
    }
}