namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.GraphicsObservable;
public class PileInfoCP
{
    public bool IsSelected { get; set; }
    public DeckRegularDict<SolitaireCard> TempList = new(); //to help with performance problems
    public DeckRegularDict<SolitaireCard> CardList { get; set; } = new();
}