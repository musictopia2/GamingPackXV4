namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.MainClasses;
public abstract class SolitaireSavedClass : IMappable, ISaveInfo
{
    public int Score { get; set; }
    [ForceClone]
    public SavedDiscardPile<SolitaireCard> Discard { get; set; } = new ();
    public string MainPileData { get; set; } = "";
    [ForceClone]
    public SavedWaste WasteData { get; set; } = new SavedWaste();
    public BasicList<int> IntDeckList { get; set; } = new();
}