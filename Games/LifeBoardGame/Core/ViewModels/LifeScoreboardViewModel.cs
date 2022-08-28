namespace LifeBoardGame.Core.ViewModels;
[InstanceGame]
public class LifeScoreboardViewModel : ScreenViewModel, IMainScreen
{
    public LifeScoreboardViewModel(IEventAggregator aggregator) : base(aggregator)
    {
    }
}