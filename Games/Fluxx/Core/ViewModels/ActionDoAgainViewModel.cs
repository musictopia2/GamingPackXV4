namespace Fluxx.Core.ViewModels;
[InstanceGame]
public partial class ActionDoAgainViewModel : BasicActionScreen
{
    public ActionDoAgainViewModel(FluxxGameContainer gameContainer,
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
    private bool CanDoAction => ActionContainer.IndexCard > -1;
    public bool CanViewCard => CanDoAction;
    [Command(EnumCommandCategory.Plain)]
    public void ViewCard()
    {
        var thisCard = ActionContainer.GetCardToDoAgain(ActionContainer.IndexCard);
        ActionContainer.CurrentDetail!.ShowCard(thisCard);
    }
    public bool CanSelectCard => CanDoAction;
    [Command(EnumCommandCategory.Plain)]
    public async Task SelectCardAsync()
    {
        await BasicActionLogic.ShowMainScreenAgainAsync();
        await FluxxEvent.DoAgainSelectedAsync(ActionContainer.IndexCard);
    }
}
