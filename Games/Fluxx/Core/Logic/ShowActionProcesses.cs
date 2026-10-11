namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class ShowActionProcesses(ActionContainer actionContainer,
    BasicActionLogic basicActionLogic,
    FluxxGameContainer gameContainer
        ) : IShowActionProcesses
{
    //this may have to update something (not sure what yet).
    async Task IShowActionProcesses.ChoseOtherCardSelectedAsync(int deck)
    {
        actionContainer.OtherHand!.SelectOneFromDeck(deck);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(.5);
        }
        await basicActionLogic.ShowMainScreenAgainAsync();
    }
    async Task IShowActionProcesses.ShowCardUseAsync(int deck)
    {
        actionContainer.TempHand!.SelectOneFromDeck(deck);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(.5);
        }
        await basicActionLogic.ShowMainScreenAgainAsync();
    }
    async Task IShowActionProcesses.ShowChosenForEverybodyGetsOneAsync(BasicList<int> selectedList, int selectedIndex)
    {
        selectedList.ForEach(thisItem =>
        {
            actionContainer.TempHand!.SelectOneFromDeck(thisItem);
        });
        actionContainer.Player1!.SelectSpecificItem(selectedIndex);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1);
        }
        await basicActionLogic.ShowMainScreenAgainAsync();
    }
    async Task IShowActionProcesses.ShowDirectionAsync(int selectedIndex)
    {
        actionContainer.Direction1!.SelectSpecificItem(selectedIndex);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(.5);
        }
        await basicActionLogic.ShowMainScreenAgainAsync();
    }
    async Task IShowActionProcesses.ShowLetsDoAgainAsync(int selectedIndex)
    {
        actionContainer.CardList1!.SelectSpecificItem(selectedIndex);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(.5);
        }
        await basicActionLogic.ShowMainScreenAgainAsync();
    }
    async Task IShowActionProcesses.ShowPlayerForCardChosenAsync(int selectedIndex)
    {
        actionContainer.IndexPlayer = selectedIndex;
        await ShowTradeHandAsync(selectedIndex);
    }
    async Task IShowActionProcesses.ShowRulesSimplifiedAsync(BasicList<int> list)
    {
        if (list.Count == 0)
        {
            return;
        }
        actionContainer.Rule1!.SelectSeveralItems(list);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1);
        }
        await basicActionLogic.ShowMainScreenAgainAsync();
    }
    async Task IShowActionProcesses.ShowRuleTrashedAsync(int selectedIndex)
    {
        actionContainer.Rule1!.SelectSpecificItem(selectedIndex);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(.75);
        }
        await basicActionLogic.ShowMainScreenAgainAsync();
    }
    private async Task ShowTradeHandAsync(int selectedIndex)
    {
        actionContainer.Player1!.SelectSpecificItem(selectedIndex);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(.75);
        }
        await basicActionLogic.ShowMainScreenAgainAsync();
    }
    async Task IShowActionProcesses.ShowTradeHandAsync(int selectedIndex)
    {
        await ShowTradeHandAsync(selectedIndex);
    }
}