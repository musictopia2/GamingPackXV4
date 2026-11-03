namespace Fluxx.Core.Logic;
public interface IPlayProcesses
{
    Task SendPlayAsync(int deck);
    Task PlayCardAsync(int deck);
    Task PlayCardAsync(FluxxCardInformation card);
    Task PlayRandomCardAsync(int deck, int player);
    Task PlayRandomCardAsync(FluxxCardInformation thisCard, int player);
    Task PlayRandomCardAsync(int deck);
    Task PlayUseTakeAsync(int deck, int player);
}