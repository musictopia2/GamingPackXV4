namespace LifeBoardGame.Core.ViewModels;
[InstanceGame]
public class GameboardViewModel : ScreenViewModel, IMainScreen
{
    public GameboardViewModel(IEventAggregator aggregator) : base(aggregator)
    {
    }
}