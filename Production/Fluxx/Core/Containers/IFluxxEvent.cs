namespace Fluxx.Core.Containers;
public interface IFluxxEvent
{
    Task CloseKeeperScreenAsync();
    Task KeepersExchangedAsync(KeeperPlayer keeperFrom, KeeperPlayer keeperTo);
    Task StealTrashKeeperAsync(KeeperPlayer currentKeeper, bool isTrashed);
    Task CardChosenToPlayAtAsync(int deck, int selectedIndex);
    Task CardToUseAsync(int deck);
    Task ChoseForEverybodyGetsOneAsync(BasicList<int> selectedList, int selectedIndex);
    Task ChosePlayerForCardChosenAsync(int selectedIndex);
    Task DirectionChosenAsync(int selectedIndex);
    Task DoAgainSelectedAsync(int selectedIndex);
    Task FirstCardRandomChosenAsync(int deck);
    Task RulesSimplifiedAsync(BasicList<int> simpleList);
    Task RuleTrashedAsync(int selectedIndex);
    Task TradeHandsAsync(int selectedIndex);
}