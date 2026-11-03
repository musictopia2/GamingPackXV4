namespace Rummy500.Core.Data;
[SingletonGame]
public class Rummy500SaveInfo : BasicSavedCardClass<Rummy500PlayerItem, RegularRummyCard>, IMappable, ISaveInfo
{
    public BasicList<SavedSet> SetList { get; set; } = [];
    public DeckRegularDict<RegularRummyCard> DiscardList { get; set; } = [];
    public bool MoreThanOne { get; set; }
}