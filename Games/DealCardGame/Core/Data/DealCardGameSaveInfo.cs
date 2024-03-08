namespace DealCardGame.Core.Data;
[SingletonGame]
public class DealCardGameSaveInfo : BasicSavedCardClass<DealCardGamePlayerItem, DealCardGameCardInformation>, IMappable, ISaveInfo
{
    public EnumGameStatus GameStatus { get; set; } = EnumGameStatus.None;
}