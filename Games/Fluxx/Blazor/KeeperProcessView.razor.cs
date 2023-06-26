namespace Fluxx.Blazor;
public partial class KeeperProcessView<K>
    where K : class
{
    [Parameter]
    public ICustomCommand? Command { get; set; }
}