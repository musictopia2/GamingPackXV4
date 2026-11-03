namespace Fluxx.Core.ViewModels;
[InstanceGame]
public class PromptViewModel(ActionContainer actionContainer, KeeperContainer keeperContainer, FluxxDelegates fluxxDelegates) : IPromptVM
{
    void IPromptVM.Initialize()
    {
        if (fluxxDelegates.CurrentScreen is null)
        {
            throw new CustomBasicException("Nobody is letting me know the action screen");
        }
        var category = fluxxDelegates.CurrentScreen.Invoke();
        if (category == EnumActionScreen.ActionScreen)
        {
            ProcessActions();
            return;
        }
        if (category == EnumActionScreen.KeeperScreen)
        {
            ProcessKeepers();
            return;
        }
        throw new CustomBasicException("Only actions and keepers are supported");
    }
    public BasicActionScreen? ActionScreen { get; private set; }
    public BasicKeeperScreen? KeeperScreen { get; private set; }
    private void ProcessActions()
    {
        KeeperScreen = null;
        ActionScreen = actionContainer.ActionCategory switch
        {
            EnumActionCategory.Rules => Resolver!.Resolve<ActionDiscardRulesViewModel>(),
            EnumActionCategory.Directions => Resolver!.Resolve<ActionDirectionViewModel>(),
            EnumActionCategory.DoAgain => Resolver!.Resolve<ActionDoAgainViewModel>(),
            EnumActionCategory.DrawUse => Resolver!.Resolve<ActionDrawUseViewModel>(),
            EnumActionCategory.Everybody1 => Resolver!.Resolve<ActionEverybodyGetsOneViewModel>(),
            EnumActionCategory.FirstRandom => Resolver!.Resolve<ActionFirstCardRandomViewModel>(),
            EnumActionCategory.TradeHands => Resolver!.Resolve<ActionTradeHandsViewModel>(),
            EnumActionCategory.UseTake => Resolver!.Resolve<ActionTakeUseViewModel>(),
            _ => throw new CustomBasicException("Cannot figure out action screen.  Rethink")
        };
    }
    private void ProcessKeepers()
    {
        ActionScreen = null;
        KeeperScreen = keeperContainer.Section switch
        {
            EnumKeeperSection.None => Resolver!.Resolve<KeeperShowViewModel>(),
            EnumKeeperSection.Trash => Resolver!.Resolve<KeeperTrashViewModel>(),
            EnumKeeperSection.Steal => Resolver!.Resolve<KeeperStealViewModel>(),
            EnumKeeperSection.Exchange => Resolver!.Resolve<KeeperExchangeViewModel>(),
            _ => throw new CustomBasicException("Not Supported"),
        };
    }
}