namespace Fluxx.Core.ViewModels;
[InstanceGame]
public partial class ActionDirectionViewModel : BasicActionScreen
{
    public ActionDirectionViewModel(FluxxGameContainer gameContainer,
        ActionContainer actionContainer,
        KeeperContainer keeperContainer,
        FluxxDelegates delegates,
        IFluxxEvent fluxxEvent,
        BasicActionLogic basicActionLogic,
        IEventAggregator aggregator
        ) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic, aggregator)
    {
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanDirection => ActionContainer.IndexDirection > -1;
    [Command(EnumCommandCategory.Plain)]
    public async Task DirectionAsync()
    {
        await BasicActionLogic.ShowMainScreenAgainAsync();
        await FluxxEvent.DirectionChosenAsync(ActionContainer.IndexDirection);
    }
}
