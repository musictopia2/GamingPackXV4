namespace BattleshipLite.Core.Data;
[SingletonGame]
public class BattleshipLiteSaveInfo : BasicSavedGameClass<BattleshipLitePlayerItem>, IMappable, ISaveInfo
{
    public EnumGameStatus GameStatus { get; set; }
}