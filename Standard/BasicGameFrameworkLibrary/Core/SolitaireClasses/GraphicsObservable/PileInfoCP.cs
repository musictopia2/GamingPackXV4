namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.GraphicsObservable;
public partial class PileInfoCP : IStateChanged
{
    
    public bool IsSelected { get; set; }
    public DeckRegularDict<SolitaireCard> TempList = new(); //to help with performance problems
    public event Action? StateChanged;
    public DeckRegularDict<SolitaireCard> CardList { get; set; } = new();
    public void RefreshPile()
    {
        StateChanged?.Invoke();
    }
}