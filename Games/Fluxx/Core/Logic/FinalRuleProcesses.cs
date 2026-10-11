namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class FinalRuleProcesses(FluxxGameContainer gameContainer, IAnalyzeProcesses processes) : IFinalRuleProcesses
{
    async Task IFinalRuleProcesses.SimplifyRulesAsync(BasicList<int> thisList)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("simplifyrules", thisList);
        }
        DeckRegularDict<RuleCard> newList = [];
        thisList.ForEach(index =>
        {
            newList.Add(gameContainer.SaveRoot!.RuleList[index + 1]);
        });
        await newList.ForEachAsync(gameContainer.DiscardRuleAsync);
        if (thisList.Count > 0)
        {
            gameContainer.RefreshRules();
        }
        gameContainer!.CurrentAction = null;
        await processes.AnalyzeQueAsync();
    }
    async Task IFinalRuleProcesses.TrashNewRuleAsync(int index)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("trashnewrule", index);
        }
        var thisRule = gameContainer.SaveRoot!.RuleList[index + 1];
        await gameContainer.DiscardRuleAsync(thisRule);
        gameContainer.RefreshRules();
        gameContainer!.CurrentAction = null;
        await processes.AnalyzeQueAsync();
    }
}
