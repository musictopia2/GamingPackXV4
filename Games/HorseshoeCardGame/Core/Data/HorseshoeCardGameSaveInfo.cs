namespace HorseshoeCardGame.Core.Data;
[SingletonGame]
public class HorseshoeCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, HorseshoeCardGameCardInformation, HorseshoeCardGamePlayerItem>, IMappable, ISaveInfo
{
    public bool FirstCardPlayed { get; set; }
}