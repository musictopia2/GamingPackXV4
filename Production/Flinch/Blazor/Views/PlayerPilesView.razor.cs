namespace Flinch.Blazor.Views;
public partial class PlayerPilesView
{
    private FlinchPlayerItem? _player;
    protected override void OnParametersSet()
    {
        if (DataContext is null)
        {
            return;
        }
        _player = DataContext!.GameContainer.PlayerList!.GetWhoPlayer();

        base.OnParametersSet();
    }
    private string DiscardTag => $"discard{_player!.NickName}";
    private string StockTag => $"stock{_player!.NickName}";
}