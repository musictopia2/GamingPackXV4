namespace Fluxx.Blazor;
public partial class KeeperButton<K>
    where K : IShowKeeperVM
{
    [CascadingParameter]
    public K? DataContext { get; set; }
    [Parameter]
    public bool StartsOnNewLine { get; set; } = false;
    private ICustomCommand ShowCommand => DataContext!.ShowKeepersCommand!;
}