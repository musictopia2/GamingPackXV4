namespace BattleshipLite.Core.Data;
public class BattleshipLitePlayerItem : SimplePlayer, IStateChanged
{
    public BattleshipCollection Ships { get; set; } = new();
    public event Action? StateChanged;
    public void UpdateShips()
    {
        if (PlayerCategory == EnumPlayerCategory.Self)
        {
            StateChanged?.Invoke();
        }
    }
}