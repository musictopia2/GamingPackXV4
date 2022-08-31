namespace Skipbo.Blazor.Views;
public partial class PlayerPilesView
{
    [CascadingParameter]
    public PlayerPilesViewModel? DataContext { get; set; }
    private SkipboPlayerItem? _player;
    protected override void OnParametersSet()
    {
        _player = DataContext!.GameContainer.PlayerList!.GetWhoPlayer();

        base.OnParametersSet();
    }
    private string DiscardTag => $"discard{_player!.NickName}";
    private string StockTag => $"stock{_player!.NickName}";
}