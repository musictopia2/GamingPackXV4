namespace CaliforniaJack.Core.Data;
[SingletonGame]
public class CaliforniaJackSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, CaliforniaJackCardInformation, CaliforniaJackPlayerItem>, IMappable, ISaveInfo
{
    public DeckRegularDict<CaliforniaJackCardInformation> CardList { get; set; } = new();
}