namespace Fluxx.Core.ViewModels;
[InstanceGame]
public partial class ActionTakeUseViewModel : BasicActionScreen
{
    public ActionTakeUseViewModel(FluxxGameContainer gameContainer,
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
        await FluxxEvent.ChosePlayerForCardChosenAsync(ActionContainer.IndexPlayer);
    }
    public bool CanChooseCard => ActionContainer.OtherHand.ObjectSelected() > 0;
    [Command(EnumCommandCategory.Plain)]
    public async Task ChooseCardAsync()
    {
        if (ActionContainer.IndexPlayer == -1)
        {
            throw new CustomBasicException("Must have the player chosen in order to use what you take from another player");
        }
        await BasicActionLogic.ShowMainScreenAgainAsync();
        await FluxxEvent.CardChosenToPlayAtAsync(ActionContainer.OtherHand.ObjectSelected(), ActionContainer.IndexPlayer);
    }
}