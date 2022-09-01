namespace Fluxx.Core.ViewModels;
[InstanceGame]
public partial class ActionTradeHandsViewModel : BasicActionScreen
{
    public ActionTradeHandsViewModel(FluxxGameContainer gameContainer,
        ActionContainer actionContainer,
        KeeperContainer keeperContainer,
        FluxxDelegates delegates,
        IFluxxEvent fluxxEvent,
        BasicActionLogic basicActionLogic,
        IEventAggregator aggregator) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic, aggregator)
    {
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanChoosePlayer => ActionContainer.CanChoosePlayer();
    [Command(EnumCommandCategory.Plain)]
    public async Task ChoosePlayerAsync()
    {
        await BasicActionLogic.ShowMainScreenAgainAsync();
        await FluxxEvent.TradeHandsAsync(ActionContainer.IndexPlayer);
    }
}