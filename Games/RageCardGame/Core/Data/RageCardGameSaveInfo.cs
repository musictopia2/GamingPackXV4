namespace RageCardGame.Core.Data;
[SingletonGame]
public class RageCardGameSaveInfo : BasicSavedTrickGamesClass<EnumColor, RageCardGameCardInformation, RageCardGamePlayerItem>, IMappable, ISaveInfo
{
    public DeckRegularDict<RageCardGameCardInformation> CardList = new();
    public int CardsToPassOut { get; set; }
    public EnumStatus Status { get; set; }
}