namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class EverybodyOneProcesses(FluxxGameContainer gameContainer, ActionContainer actionContainer, IAnalyzeProcesses processes) : IEverybodyOneProcesses
{
    async Task IEverybodyOneProcesses.EverybodyGetsOneAsync(BasicList<int> thisList, int selectedIndex)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            BasicList<int> sendList = [.. thisList, selectedIndex];
            await gameContainer.Network!.SendAllAsync("everybodygetsone", sendList);
        }
        int player = actionContainer.GetPlayerIndex(selectedIndex);
        var newList = thisList.Select(items => new PreviousCard { Deck = items, Player = player }).ToBasicList();
        gameContainer!.EverybodyGetsOneList.AddRange(newList);
        int oldCount = gameContainer.TempActionHandList.Count;
        gameContainer.TempActionHandList.RemoveGivenList(thisList);
        actionContainer.SetUpGoals();
        if (gameContainer.TempActionHandList.Count == oldCount)
        {
            throw new CustomBasicException("Did not remove from temphand");
        }
        if (gameContainer.CanLoadEverybodyGetsOne() == false)
        {
            await LastCardsForEverybodyGetsOneAsync();
            return;
        }
        await processes.AnalyzeQueAsync();
    }
    private async Task LastCardsForEverybodyGetsOneAsync()
    {
        int extras = gameContainer!.IncreaseAmount();
        int maxs = extras + 1;
        foreach (var thisPlayer in gameContainer.PlayerList!)
        {
            if (gameContainer.EverybodyGetsOneList.Count(items => items.Player == thisPlayer.Id) < maxs)
            {
                var tempList = gameContainer.TempActionHandList.Select(items => new PreviousCard { Deck = items, Player = thisPlayer.Id }).ToBasicList();
                gameContainer.EverybodyGetsOneList.AddRange(tempList);
                gameContainer.TempActionHandList.Clear();
                await FinalEverybodyGetsOneAsync();
                return;
            }
        }
        throw new CustomBasicException("Wrong");
    }
    private async Task FinalEverybodyGetsOneAsync()
    {
        gameContainer!.EverybodyGetsOneList.ForEach(thisTemp =>
        {
            var thisPlayer = gameContainer.PlayerList![thisTemp.Player];
            var thisCard = gameContainer.DeckList!.GetSpecificItem(thisTemp.Deck);
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            thisPlayer.MainHandList.Add(thisCard);
        });
        gameContainer.EverybodyGetsOneList.Clear();
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetSelf();
        gameContainer.SortCards!();
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        gameContainer.CurrentAction = null;
        await processes.AnalyzeQueAsync();
    }
}
