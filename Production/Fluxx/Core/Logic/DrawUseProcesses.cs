namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class DrawUseProcesses(FluxxGameContainer gameContainer, IAnalyzeProcesses analyzeProcesses) : IDrawUseProcesses
{
    async Task IDrawUseProcesses.DrawUsedAsync(int deck)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("drawuse", deck);
        }
        gameContainer!.TempActionHandList.RemoveSpecificItem(deck);
        var thisCard = gameContainer.DeckList!.GetSpecificItem(deck);
        gameContainer.QuePlayList.Add(thisCard);
        if (gameContainer.TempActionHandList.Count == 1)
        {
            if (gameContainer.CurrentAction!.Deck == EnumActionMain.Draw3Play2OfThem)
            {
                gameContainer.SaveRoot!.SavedActionData.TempDiscardList.Add(gameContainer.TempActionHandList.Single());
            }
            else
            {
                thisCard = gameContainer.DeckList.GetSpecificItem(gameContainer.TempActionHandList.Single());
                gameContainer.QuePlayList.Add(thisCard);
            }
            gameContainer.TempActionHandList.Clear();
            gameContainer.CurrentAction = null;
            await analyzeProcesses.AnalyzeQueAsync();
            return;
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}
