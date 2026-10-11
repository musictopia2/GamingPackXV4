namespace SnagCardGame.Core.Data;
[SingletonGame]
public class SnagCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, SnagCardGameCardInformation, SnagCardGamePlayerItem>, IMappable, ISaveInfo
{
    public EnumStatusList GameStatus { get; set; }
    public DeckRegularDict<SnagCardGameCardInformation> CardList { get; set; } = [];
    public DeckRegularDict<SnagCardGameCardInformation> BarList { get; set; } = [];
    public bool FirstCardPlayed { get; set; }
}