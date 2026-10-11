namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class DiscardProcesses(FluxxGameContainer gameContainer, IAnalyzeProcesses analyzeProcesses) : IDiscardProcesses
{
    public async Task DiscardFromHandAsync(BasicList<int> list)
    {
        var newList = list.GetFluxxCardListFromDeck(gameContainer);
        await DiscardFromHandAsync(newList);
    }
    public async Task DiscardFromHandAsync(FluxxCardInformation thisCard)
    {
        gameContainer.RemoveFromHandOnly(thisCard);
        await gameContainer.AnimatePlayAsync!(thisCard);
    }
    public async Task DiscardFromHandAsync(IDeckDict<FluxxCardInformation> list)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("discardfromhand", list.GetDeckListFromObjectList());
        }
        await list.ForEachAsync(async thisCard =>
        {
            await DiscardFromHandAsync(thisCard);
            if (gameContainer.Test!.NoAnimations == false)
            {
                await gameContainer.Delay!.DelaySeconds(.1);
            }
        });
        await analyzeProcesses.AnalyzeQueAsync();
    }
    public async Task DiscardFromHandAsync(int deck)
    {
        var thisCard = gameContainer.SingleInfo!.MainHandList.GetSpecificItem(deck);
        await DiscardFromHandAsync(thisCard);
    }
    public async Task DiscardGoalAsync(int deck)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("discardgoal", deck);
        }
        var thisGoal = (GoalCard)gameContainer.DeckList!.GetSpecificItem(deck);
        await DiscardGoalAsync(thisGoal);
    }
    public async Task DiscardGoalAsync(GoalCard thisCard)
    {
        gameContainer.SaveRoot!.GoalList.RemoveObjectByDeck((int)thisCard.Deck);
        await gameContainer.AnimatePlayAsync!(thisCard);
    }
    public async Task DiscardKeeperAsync(int deck)
    {
        var thisKeeper = (KeeperCard)gameContainer.DeckList!.GetSpecificItem(deck);
        await DiscardKeeperAsync(thisKeeper);
    }
    public async Task DiscardKeeperAsync(FluxxCardInformation thisCard)
    {
        gameContainer.SingleInfo!.KeeperList.RemoveObjectByDeck(thisCard.Deck);
        await gameContainer.AnimatePlayAsync!(thisCard);
    }
    public async Task DiscardKeepersAsync(IDeckDict<FluxxCardInformation> list)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("discardkeepers", list.GetDeckListFromObjectList());
        }
        await list.ForEachAsync(async thisCard =>
        {
            await DiscardKeeperAsync(thisCard);
            if (gameContainer.Test!.NoAnimations == false)
            {
                await gameContainer.Delay!.DelaySeconds(.1);
            }
        });
        await analyzeProcesses.AnalyzeQueAsync();
    }
    public async Task DiscardKeepersAsync(BasicList<int> list)
    {
        var newList = list.GetFluxxCardListFromDeck(gameContainer);
        await DiscardKeepersAsync(newList);
    }
}
