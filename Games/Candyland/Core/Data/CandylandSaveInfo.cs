namespace Candyland.Core.Data;
[SingletonGame]
public class CandylandSaveInfo : BasicSavedGameClass<CandylandPlayerItem>, IMappable, ISaveInfo, ISavedCardList<CandylandCardData>
{
    public bool DidDraw { get; set; }
    public DeckRegularDict<CandylandCardData> CardList { get; set; } = new();
    public CandylandCardData? CurrentCard { get; set; }
}