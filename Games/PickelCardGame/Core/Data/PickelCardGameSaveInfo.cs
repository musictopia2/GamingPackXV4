namespace PickelCardGame.Core.Data;
[SingletonGame]
public class PickelCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, PickelCardGameCardInformation, PickelCardGamePlayerItem>, IMappable, ISaveInfo
{
    public EnumStatusList GameStatus { get; set; }
    public int HighestBid { get; set; }
    public int WonSoFar { get; set; }
    public BasicList<PickelCardGameCardInformation> CardList { get; set; } = new(); //this holds the entire list of all cards played.
}