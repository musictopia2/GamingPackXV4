namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.GraphicsObservable;
public class PileInfoCP
{
    public bool IsSelected { get; set; }
    public DeckRegularDict<SolitaireCard> TempList = new(); //this is needed because otherwise the piles has performance problems.
    public DeckRegularDict<SolitaireCard> CardList { get; set; } = new(); //iffy if we really need both now.
}