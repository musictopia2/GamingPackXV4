namespace Fluxx.Blazor;
public partial class KeeperButton<K>
    where K : IShowKeeperVM
{
    [Parameter]
    public bool StartsOnNewLine { get; set; } = false;
    private ICustomCommand ShowCommand => DataContext!.ShowKeepersCommand!;
}