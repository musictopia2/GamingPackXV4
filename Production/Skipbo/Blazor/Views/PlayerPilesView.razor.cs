namespace Skipbo.Blazor.Views;
public partial class PlayerPilesView
{
    private SkipboPlayerItem? _player;
    protected override void OnParametersSet()
    {
        //hopefully still works (?)
        if (DataContext is null)
        {
            return; //if its null, ignore.
        }
        _player = DataContext!.GameContainer.PlayerList!.GetWhoPlayer();
        base.OnParametersSet();
    }
    private string DiscardTag => $"discard{_player!.NickName}";
    private string StockTag => $"stock{_player!.NickName}";
}