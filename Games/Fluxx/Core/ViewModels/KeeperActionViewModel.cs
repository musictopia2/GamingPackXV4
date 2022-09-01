namespace Fluxx.Core.ViewModels;
public abstract partial class KeeperActionViewModel : BasicKeeperScreen
{
    private readonly IFluxxEvent _fluxxEvent;
    private readonly IToast _toast;
    public abstract string ButtonText { get; }
    public KeeperActionViewModel(FluxxGameContainer gameContainer, 
        KeeperContainer keeperContainer, 
        IFluxxEvent fluxxEvent, 
        IEventAggregator aggregator,
        IToast toast
        ) : base(gameContainer, keeperContainer, aggregator)
    {
        _fluxxEvent = fluxxEvent;
        _toast = toast;
        keeperContainer.ButtonText = ButtonText;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.Plain)]
    public async Task ProcessAsync()
    {
        var thisList = GetSelectList();
        if (thisList.Count == 0)
        {
            _toast.ShowUserErrorToast("Must choose a keeper");
            return;
        }
        if (KeeperContainer.Section == EnumKeeperSection.Exchange)
        {
            if (thisList.Count != 2)
            {
                _toast.ShowUserErrorToast("Must choose a keeper from yourself and from another player for exchange");
                return;
            }
            if (ContainsCurrentPlayer(thisList) == false)
            {
                _toast.ShowUserErrorToast("Must choose a keeper from your keeper list in order to exchange");
                return;
            }
            KeeperPlayer keeperFrom = new() { Player = GameContainer.WhoTurn };
            var thisHand = GetCurrentPlayerKeeperHand(thisList);
            keeperFrom.Card = thisHand.ObjectSelected();
            if (keeperFrom.Card == 0)
            {
                throw new CustomBasicException("Keeper was never selected for current player");
            }
            thisList.RemoveSpecificItem(thisHand);
            KeeperPlayer keeperTo = new();
            keeperTo.Player = GetPlayerOfKeeperHand(thisList.Single());
            keeperTo.Card = thisList.Single().ObjectSelected();
            if (keeperTo.Card == 0)
            {
                throw new CustomBasicException("Keeper was never selected for player");
            }
            await _fluxxEvent.KeepersExchangedAsync(keeperFrom, keeperTo);
            return;
        }
        if (thisList.Count != 1)
        {
            _toast.ShowUserErrorToast("Must choose only one keeper");
            return;
        }
        int index = GetPlayerOfKeeperHand(thisList.Single());
        bool isTrashed;
        if (index == GameContainer.WhoTurn && KeeperContainer.Section != EnumKeeperSection.Trash)
        {
            _toast.ShowUserErrorToast("Cannot steal a keeper from yourself");
            return;
        }
        isTrashed = KeeperContainer.Section == EnumKeeperSection.Trash;
        KeeperPlayer tempKeep = new();
        tempKeep.Player = index;
        tempKeep.Card = thisList.Single().ObjectSelected();
        if (tempKeep.Card == 0)
        {
            throw new CustomBasicException("Keeper was never selected");
        }
        await _fluxxEvent.StealTrashKeeperAsync(tempKeep, isTrashed);
    }
    private int GetPlayerOfKeeperHand(HandObservable<KeeperCard> thisHand)
    {
        int index = KeeperContainer.KeeperHandList!.IndexOf(thisHand);
        return index + 1;
    }
    private bool ContainsCurrentPlayer(BasicList<HandObservable<KeeperCard>> thisList)
    {
        return thisList.Any(items => GetPlayerOfKeeperHand(items) == GameContainer.WhoTurn);
    }
    private HandObservable<KeeperCard> GetCurrentPlayerKeeperHand(BasicList<HandObservable<KeeperCard>> thisList)
    {
        return thisList.Single(items => GetPlayerOfKeeperHand(items) == GameContainer.WhoTurn);
    }
    private BasicList<HandObservable<KeeperCard>> GetSelectList()
    {
        return KeeperContainer.KeeperHandList.Where(items => items.ObjectSelected() > 0).ToBasicList();
    }
}
