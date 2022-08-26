
namespace LottoDominos.Core.Data;
[SingletonGame]
public class LottoDominosSaveInfo : BasicSavedGameClass<LottoDominosPlayerItem>, IMappable, ISaveInfo
{
    public DeckRegularDict<SimpleDominoInfo> ComputerList { get; set; } = new();
    public DeckRegularDict<SimpleDominoInfo>? BoardDice { get; set; }
    public EnumStatus GameStatus { get; set; }
}