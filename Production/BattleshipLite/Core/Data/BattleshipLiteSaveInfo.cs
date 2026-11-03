namespace BattleshipLite.Core.Data;
[SingletonGame]
[UseStateChanged]
public partial class BattleshipLiteSaveInfo : BasicSavedGameClass<BattleshipLitePlayerItem>, IMappable, ISaveInfo
{
    [StateProperty]
    public partial EnumGameStatus GameStatus { get; set; }
}