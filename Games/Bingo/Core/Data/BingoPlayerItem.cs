namespace Bingo.Core.Data;
public class BingoPlayerItem : SimplePlayer
{
    public PlayerBingo BingoList { get; set; } = new();
}