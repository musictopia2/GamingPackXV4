namespace BattleshipLite.Core.Data;
public class BattleshipLitePlayerItem : SimplePlayer
{
    public BattleshipCollection Ships { get; set; } = new();
}