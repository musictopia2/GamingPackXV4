namespace Spades4Player.Core.Data;
[SingletonGame]
public class Spades4PlayerSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, Spades4PlayerCardInformation, Spades4PlayerPlayerItem>, IMappable, ISaveInfo, ITrickStatusSavedClass
{
    public EnumTrickStatus TrickStatus { get; set; }
    public EnumGameStatus GameStatus { get; set; }
}