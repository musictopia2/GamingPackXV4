namespace TicTacToe.Core.Data;
[SingletonGame]
public class TicTacToeSaveInfo : BasicSavedGameClass<TicTacToePlayerItem>, IMappable, ISaveInfo
{
    public TicTacToeCollection GameBoard { get; set; } = new();
}