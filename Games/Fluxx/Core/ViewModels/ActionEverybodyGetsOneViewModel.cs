namespace Fluxx.Core.ViewModels;
[InstanceGame]
public partial class ActionEverybodyGetsOneViewModel : BasicActionScreen
{
    private readonly IToast _toast;
    public ActionEverybodyGetsOneViewModel(FluxxGameContainer gameContainer,
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
    public bool CanGiveCards => ActionContainer.IndexPlayer > -1;
    [Command(EnumCommandCategory.Plain)]
    public async Task GiveCardsAsync()
    {
        BasicList<int> thisList;
        if (ActionContainer.TempHand!.AutoSelect == EnumHandAutoType.SelectOneOnly)
        {
            if (ActionContainer.TempHand.ObjectSelected() == 0)
            {
                _toast.ShowUserErrorToast("Must choose a card to give to a player");
                return;
            }
            thisList = new() { ActionContainer.TempHand.ObjectSelected() };
            await BasicActionLogic.ShowMainScreenAgainAsync();
            await FluxxEvent.ChoseForEverybodyGetsOneAsync(thisList, ActionContainer.IndexPlayer);
            return;
        }
        if (ActionContainer.TempHand.HowManySelectedObjects > 2)
        {
            _toast.ShowUserErrorToast("Cannot choose more than 2 cards to give to player");
            return;
        }
        int index = ActionContainer.GetPlayerIndex(ActionContainer.IndexPlayer);
        int howManySoFar = GameContainer.EverybodyGetsOneList.Count(items => items.Player == index);
        howManySoFar += ActionContainer.TempHand.HowManySelectedObjects;
        int extras = GameContainer.IncreaseAmount();
        int mosts = extras + 1;
        if (howManySoFar > mosts)
        {
            _toast.ShowUserErrorToast($"Cannot choose more than 2 cards each for the player.  So far; you chose {howManySoFar} cards.");
            return;
        }
        var finalList = ActionContainer.TempHand.ListSelectedObjects();
        thisList = finalList.GetDeckListFromObjectList();
        await BasicActionLogic.ShowMainScreenAgainAsync();
        await FluxxEvent.ChoseForEverybodyGetsOneAsync(thisList, ActionContainer.IndexPlayer);
    }
}