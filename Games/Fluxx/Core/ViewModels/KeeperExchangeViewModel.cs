namespace Fluxx.Core.ViewModels;
[InstanceGame]
public class KeeperExchangeViewModel : KeeperActionViewModel
{
    public KeeperExchangeViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent, IEventAggregator aggregator, IToast toast) : base(gameContainer, keeperContainer, fluxxEvent, aggregator, toast)
    {
    }
    public override string ButtonText => "Exchange A Keeper";
}