namespace Fluxx.Core.ViewModels;
[InstanceGame]
public class KeeperTrashViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent, IToast toast) : KeeperActionViewModel(gameContainer, keeperContainer, fluxxEvent, toast)
{
    public override string ButtonText => "Trash A Keeper";
}