namespace CribbagePatience.Core.Data;
[SingletonGame]
public class CribbagePatienceSaveInfo : IMappable
{
    public BasicList<int> DeckList { get; set; } = new();
    public BasicList<int> ScoreList { get; set; } = new();
    public SavedDiscardPile<CribbageCard>? StartCard { get; set; }
    private BasicList<ScoreHandCP> _handScores = new();
    public BasicList<ScoreHandCP> HandScores
    {
        get { return _handScores; }
        set
        {
            if (SetProperty(ref _handScores, value))
            {
                Publish();
            }
        }
    }
    private void Publish()
    {
        if (_aggregator == null)
        {
            return;
        }
        _aggregator.Publish(new HandScoresEventModel(_handScores));
    }
    private IEventAggregator? _aggregator;
    public void Load(IEventAggregator aggregator)
    {
        _aggregator = aggregator;
        Publish();
    }
}