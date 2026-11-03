namespace Fluxx.Core.Logic;
public interface IDiscardProcesses
{
    Task DiscardFromHandAsync(BasicList<int> list);
    Task DiscardFromHandAsync(FluxxCardInformation thisCard);
    Task DiscardFromHandAsync(IDeckDict<FluxxCardInformation> list);
    Task DiscardFromHandAsync(int deck);
    Task DiscardGoalAsync(int deck);
    Task DiscardGoalAsync(GoalCard thisCard);
    Task DiscardKeeperAsync(int deck);
    Task DiscardKeeperAsync(FluxxCardInformation thisCard);
    Task DiscardKeepersAsync(IDeckDict<FluxxCardInformation> list);
    Task DiscardKeepersAsync(BasicList<int> list);
}