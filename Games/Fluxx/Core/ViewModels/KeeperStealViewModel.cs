namespace Fluxx.Core.ViewModels;
[InstanceGame]
public class KeeperStealViewModel : KeeperActionViewModel
{
    public KeeperStealViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent, IEventAggregator aggregator, IToast toast) : base(gameContainer, keeperContainer, fluxxEvent, aggregator, toast)
    {
    }
    public override string ButtonText => "Steal A Keeper";
}