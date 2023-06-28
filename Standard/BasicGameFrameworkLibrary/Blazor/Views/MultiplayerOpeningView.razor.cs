
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
    private readonly BasicList<LabelGridModel> _labels = new();
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
}