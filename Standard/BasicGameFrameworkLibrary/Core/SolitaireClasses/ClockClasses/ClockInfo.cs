namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.ClockClasses;
public class ClockInfo
{
    public bool IsEnabled { get; set; } = true;
    public DeckRegularDict<SolitaireCard> CardList { get; set; } = new DeckRegularDict<SolitaireCard>();
    public bool IsSelected { get; set; }
    [JsonIgnore]
    public PointF Location { get; set; }
    public int NumberGuide { get; set; }
    public int LeftGuide { get; set; }
}