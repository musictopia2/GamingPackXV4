namespace Fluxx.Core.ViewModels;
[InstanceGame]
public class KeeperTrashViewModel : KeeperActionViewModel
{
    public KeeperTrashViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent, IEventAggregator aggregator, IToast toast) : base(gameContainer, keeperContainer, fluxxEvent, aggregator, toast)
    {
    }
    public override string ButtonText => "Trash A Keeper";
}