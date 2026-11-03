namespace DiceDominos.Core.Data;
[SingletonGame]
public class DiceDominosSaveInfo : BasicSavedDiceClass<SimpleDice, DiceDominosPlayerItem>, IMappable, ISaveInfo
{
    public DeckRegularDict<SimpleDominoInfo>? BoardDice { get; set; }
    public bool HasRolled { get; set; }
    public bool DidHold { get; set; }
}