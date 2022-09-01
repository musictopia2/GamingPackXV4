namespace Fluxx.Core.Containers;
[SingletonGame]
public class FluxxDelegates
{
    internal Func<ActionContainer, Task>? LoadProperActionScreenAsync { get; set; }
    internal Func<Task>? LoadMainScreenAsync { get; set; }
    internal Func<KeeperContainer, Task>? LoadKeeperScreenAsync { get; set; }
    internal Func<EnumActionScreen>? CurrentScreen { get; set; }
    internal Action? RefreshEnables { get; set; }
}