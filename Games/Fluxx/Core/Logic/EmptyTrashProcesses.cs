namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class EmptyTrashProcesses(FluxxGameContainer gameContainer, FluxxVMData model, IAnalyzeProcesses processes, IToast toast) : IEmptyTrashProcesses
{
    public async Task EmptyTrashAsync()
    {
        toast.ShowInfoToast("Empty the trash was played.  Therefore; the cards are being reshuffled");
        if (gameContainer.BasicData!.MultiPlayer && gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData) == false)
        {
            gameContainer.Network!.IsEnabled = true;
            return;
        }
        var thisList = model.Pile1!.DiscardList();
        thisList.AddRange(model.Deck1!.DeckList());
        thisList.ShuffleList();
        if (gameContainer.BasicData.MultiPlayer)
        {
            await gameContainer.Network!.SendAllAsync("emptytrash", thisList.GetDeckListFromObjectList());
        }
        await FinishEmptyTrashAsync(thisList);
    }
    public async Task FinishEmptyTrashAsync(IEnumerableDeck<FluxxCardInformation> cardList)
    {
        model!.Deck1!.OriginalList(cardList);
        model.Pile1!.CardsReshuffled();
        await processes.AnalyzeQueAsync();
    }
}
