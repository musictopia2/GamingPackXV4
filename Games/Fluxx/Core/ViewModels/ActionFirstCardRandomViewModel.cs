namespace Fluxx.Core.ViewModels;
[InstanceGame]
public partial class ActionFirstCardRandomViewModel : BasicActionScreen
{
    private readonly IToast _toast;
    public ActionFirstCardRandomViewModel(FluxxGameContainer gameContainer,
        ActionContainer actionContainer,
        KeeperContainer keeperContainer,
        FluxxDelegates delegates,
        IFluxxEvent fluxxEvent,
        BasicActionLogic basicActionLogic,
        IToast toast,
        IEventAggregator aggregator) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic, aggregator)
    {
        _toast = toast;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.Plain)]
    public async Task ChooseCardAsync()
    {
        if (ActionContainer.OtherHand!.ObjectSelected() == 0)
        {
            _toast.ShowUserErrorToast("Must choose a card");
            return;
        }
        await BasicActionLogic.ShowMainScreenAgainAsync();
        await FluxxEvent.FirstCardRandomChosenAsync(ActionContainer.OtherHand.ObjectSelected());
    }
}