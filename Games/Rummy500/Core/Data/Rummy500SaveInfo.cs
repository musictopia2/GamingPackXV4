namespace Rummy500.Core.Data;
[SingletonGame]
public class Rummy500SaveInfo : BasicSavedCardClass<Rummy500PlayerItem, RegularRummyCard>, IMappable, ISaveInfo
{
    public BasicList<SavedSet> SetList { get; set; } = new();
    public DeckRegularDict<RegularRummyCard> DiscardList { get; set; } = new();
    public bool MoreThanOne { get; set; }
}