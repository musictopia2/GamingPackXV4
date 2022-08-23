namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.ClockClasses;
public class ClockInfo
{
    public bool IsEnabled { get; set; } = true;
    public DeckRegularDict<SolitaireCard> CardList { get; set; } = new DeckRegularDict<SolitaireCard>(); //maybe this one is okay.
    public bool IsSelected { get; set; }
    [JsonIgnore] //i think needs to ignore.  hopefully that will work.
    public PointF Location { get; set; } //once set, will not change.
    public int NumberGuide { get; set; }
    public int LeftGuide { get; set; }
}