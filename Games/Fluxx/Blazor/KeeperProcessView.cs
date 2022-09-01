namespace Fluxx.Blazor;
public abstract class KeeperProcessView<K> : KeeperBaseView<K>
    where K : class
{
    protected override EnumKeeperCategory KeeperCategory => EnumKeeperCategory.Process;
}