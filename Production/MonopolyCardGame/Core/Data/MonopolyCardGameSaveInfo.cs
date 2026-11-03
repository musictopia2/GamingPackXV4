namespace MonopolyCardGame.Core.Data;
[SingletonGame]
public class MonopolyCardGameSaveInfo : BasicSavedCardClass<MonopolyCardGamePlayerItem, MonopolyCardGameCardInformation>, IMappable, ISaveInfo
{
    public EnumWhatStatus GameStatus { get; set; }
    public EnumManuelStatus ManuelStatus { get; set; }
    public int WhoWentOut { get; set; }
}