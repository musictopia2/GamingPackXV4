namespace SorryDicedGame.Core.Data;
[SingletonGame]
public class SorryDicedGameSaveInfo : BasicSavedGameClass<SorryDicedGamePlayerItem>, IMappable, ISaveInfo
{
    public BasicList<SorryDiceModel> DiceList { get; set; } = [];
    public BasicList<BoardModel> BoardList { get; set; } = [];
}