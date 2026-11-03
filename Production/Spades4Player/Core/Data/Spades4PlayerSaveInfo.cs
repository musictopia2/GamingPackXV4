namespace Spades4Player.Core.Data;
[SingletonGame]
[UseStateChanged]
public partial class Spades4PlayerSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, Spades4PlayerCardInformation, Spades4PlayerPlayerItem>, IMappable, ISaveInfo, ITrickStatusSavedClass
{
    public EnumTrickStatus TrickStatus { get; set; }
    [StateProperty]
    public partial EnumGameStatus GameStatus { get; set; }
}