namespace Fluxx.Core.ViewModels;
[InstanceGame]
public class KeeperExchangeViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent, IToast toast)
    : KeeperActionViewModel(gameContainer, keeperContainer, fluxxEvent, toast)
{
    public override string ButtonText => "Exchange A Keeper";
}