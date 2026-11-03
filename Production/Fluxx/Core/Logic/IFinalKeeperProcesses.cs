namespace Fluxx.Core.Logic;
public interface IFinalKeeperProcesses
{
    Task ProcessTrashStealKeeperAsync(KeeperPlayer thisKeeper, bool isTrashed);
    Task ProcessExchangeKeepersAsync(KeeperPlayer keeperFrom, KeeperPlayer keeperTo);
}