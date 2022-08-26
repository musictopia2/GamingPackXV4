namespace AlternationSolitaire.Core.Data;
[SingletonGame]
public class ScoreModel : IScoreData
{
    private int _score;
    private readonly IEventAggregator _aggregator;
    public int Score
    {
        get { return _score; }
        set
        {
            if (SetProperty(ref _score, value))
            {
                //can decide what to do when property changes
                DoPublish(this);
            }
        }
    }
    private void DoPublish(IScoreData data)
    {
        _aggregator.Publish(data);
    }
    public ScoreModel(IEventAggregator aggregator)
    {
        _aggregator = aggregator;
    }
}