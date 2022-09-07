namespace BasicGameFrameworkLibrary.Core.DrawableListsObservable;
public class SavedDiscardPile<D> where D : IDeckObject, new()
{
    public D CurrentCard { get; set; } = new D();
    public DeckRegularDict<D> PileList { get; set; } = new();
}