namespace HeapSolitaire.Core.Data;
[SingletonGame]
public class HeapSolitaireSaveInfo : IMappable
{
    public BasicList<int> DeckList { get; set; } = new();
    public BasicList<BasicPileInfo<HeapSolitaireCardInfo>> WasteData { get; set; } = new();
    public BasicList<BasicPileInfo<HeapSolitaireCardInfo>> MainPiles { get; set; } = new();
    public int PreviousSelected { get; set; } = -1; //has to show at first that none is selected.
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