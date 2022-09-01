namespace Fluxx.Core.ViewModels;
[InstanceGame]
public partial class ActionDrawUseViewModel : BasicActionScreen
{
    private readonly IToast _toast;
    public ActionDrawUseViewModel(FluxxGameContainer gameContainer,
        ActionContainer actionContainer,
        KeeperContainer keeperContainer,
        FluxxDelegates delegates,
        IFluxxEvent fluxxEvent,
        BasicActionLogic basicActionLogic,
        IEventAggregator aggregator,
        IToast toast
        ) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic, aggregator)
    {
        _toast = toast;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.Plain)]
    public async Task DrawUseAsync()
    {
        if (ActionContainer.TempHand!.ObjectSelected() == 0)
        {
            _toast.ShowUserErrorToast("Must choose a card");
            return;
        }
        await BasicActionLogic.ShowMainScreenAgainAsync();
        await FluxxEvent.CardToUseAsync(ActionContainer.TempHand.ObjectSelected());
    }
}
