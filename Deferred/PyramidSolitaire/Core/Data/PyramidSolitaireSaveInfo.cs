namespace PyramidSolitaire.Core.Data;
[SingletonGame]
public class PyramidSolitaireSaveInfo : IMappable
{
    public BasicList<int> DeckList { get; set; } = new();
    public SavedDiscardPile<SolitaireCard>? DiscardPileData { get; set; }
    public SavedDiscardPile<SolitaireCard>? CurrentPileData { get; set; }
    public SavedTriangle? TriangleData { get; set; }
    public DeckRegularDict<SolitaireCard> PlayList { get; set; } = new DeckRegularDict<SolitaireCard>();
    public SolitaireCard RecentCard { get; set; } = new SolitaireCard();
    private int _score;
    public int Score
    {
        get { return _score; }
        set
        {
            if (SetProperty(ref _score, value))
            {
                Publish();
            }
        }
    }
    public int FirstDeck { get; set; }
    public int SecondDeck { get; set; }
    private IEventAggregator? _aggregator;
    private void Publish()
    {
        if (_aggregator == null)
        {
            return;
        }
        _aggregator.Publish(new ScoreEventModel(Score));
    }
    public void Load(IEventAggregator aggregator)
    {
        _aggregator = aggregator;
        Publish();
    }
}