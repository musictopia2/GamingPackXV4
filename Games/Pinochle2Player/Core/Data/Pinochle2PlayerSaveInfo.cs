namespace Pinochle2Player.Core.Data;
[SingletonGame]
public class Pinochle2PlayerSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem>, IMappable, ISaveInfo
{
    public DeckRegularDict<Pinochle2PlayerCardInformation> CardList { get; set; } = new();
    public int GameOverAt { get; set; } = 1000;
    public bool ChooseToMeld { get; set; }
    public BasicList<MeldClass> MeldList { get; set; } = new();
    public string StartMessage { get; set; } = "";
}