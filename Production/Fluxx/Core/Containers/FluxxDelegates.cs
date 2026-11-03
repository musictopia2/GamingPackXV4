namespace Fluxx.Core.Containers;
[SingletonGame]
public class FluxxDelegates
{
    internal Func<Task>? LoadActionScreenAsync { get; set; }
    internal Func<Task>? LoadMainScreenAsync { get; set; }
    internal Func<Task>? LoadKeeperScreenAsync { get; set; }
    internal Func<EnumActionScreen>? CurrentScreen { get; set; }
    internal Action? RefreshEnables { get; set; }
}