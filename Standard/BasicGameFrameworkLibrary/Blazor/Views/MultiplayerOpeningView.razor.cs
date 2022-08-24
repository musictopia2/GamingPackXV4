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
    //private bool _disposedValue;
    protected override void OnInitialized()
    {
        //DataContext!.CommandContainer.AddAction(ShowChange); //means this is necessary.
        _labels.Clear();
        _labels.AddLabel("Players Connected", nameof(IMultiplayerOpeningViewModel.ClientsConnected))
            .AddLabel("Previous Players", nameof(IMultiplayerOpeningViewModel.PreviousNonComputerNetworkedPlayers));
        base.OnInitialized();
    }
    private void ShowChange()
    {
        InvokeAsync(() => StateHasChanged());
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
    //protected virtual void Dispose(bool disposing)
    //{
    //    if (!_disposedValue)
    //    {
    //        if (disposing)
    //        {
    //            DataContext!.CommandContainer.RemoveAction(ShowChange);
    //        }
    //        _disposedValue = true;
    //    }
    //}
    //public void Dispose()
    //{
    //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //    Dispose(disposing: true);
    //    GC.SuppressFinalize(this);
    //}
}