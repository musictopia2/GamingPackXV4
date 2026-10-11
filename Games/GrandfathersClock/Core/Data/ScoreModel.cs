namespace GrandfathersClock.Core.Data;
[SingletonGame]
public class ScoreModel(IEventAggregator aggregator) : IScoreData, IClockVM
{
    private int _score;

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
        aggregator.Publish(data);
    }

    Task IClockVM.ClockClickedAsync(int index)
    {
        throw new CustomBasicException("Rethinking may be necessary");
    }
}