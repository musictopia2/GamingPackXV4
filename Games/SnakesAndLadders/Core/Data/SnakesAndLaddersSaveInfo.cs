namespace SnakesAndLadders.Core.Data;
[SingletonGame]
public class SnakesAndLaddersSaveInfo : BasicSavedGameClass<SnakesAndLaddersPlayerItem>, IMappable, ISaveInfo
{
    public DiceList<SimpleDice> DiceList { get; set; } = new();
    public bool HasRolled { get; set; }
}