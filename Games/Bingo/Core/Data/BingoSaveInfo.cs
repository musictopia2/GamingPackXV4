namespace Bingo.Core.Data;
[SingletonGame]
public class BingoSaveInfo : BasicSavedGameClass<BingoPlayerItem>, IMappable, ISaveInfo
{
    public GameBoardCP BingoBoard { get; set; } = new();
    public Dictionary<int, BingoItem> CallList { get; set; } = new();
}