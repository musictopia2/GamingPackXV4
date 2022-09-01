namespace RoundsCardGame.Core.Data;
[SingletonGame]
public class RoundsCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, RoundsCardGameCardInformation, RoundsCardGamePlayerItem>, IMappable, ISaveInfo
{
    public int PartRound { get; set; } //from 1 to 5.
    public DeckRegularDict<RoundsCardGameCardInformation> CardList { get; set; } = new();
}