namespace GalaxyCardGame.Core.Data;
[SingletonGame]
[UseStateChanged]
public partial class GalaxyCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, GalaxyCardGameCardInformation, GalaxyCardGamePlayerItem>, IMappable, ISaveInfo
{
    public EnumGameStatus GameStatus { get; set; }
    [StateProperty]
    public partial GalaxyCardGameCardInformation WinningCard { get; set; } = new ();
    public int LastWin { get; set; }
}