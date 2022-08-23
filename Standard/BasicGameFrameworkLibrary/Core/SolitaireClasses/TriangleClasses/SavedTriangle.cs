namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.TriangleClasses;
public class SavedTriangle
{
    public bool InPlay { get; set; }
    public BasicList<SolitaireCard> CardList { get; set; } = new();
}