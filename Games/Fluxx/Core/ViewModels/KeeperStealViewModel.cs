namespace Fluxx.Core.ViewModels;
[InstanceGame]
public class KeeperStealViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent, IToast toast) : KeeperActionViewModel(gameContainer, keeperContainer, fluxxEvent, toast)
{
    public override string ButtonText => "Steal A Keeper";
}