namespace Fluxx.Core.Logic;
public interface IEmptyTrashProcesses
{
    Task EmptyTrashAsync();
    Task FinishEmptyTrashAsync(IEnumerableDeck<FluxxCardInformation> cardList);
}