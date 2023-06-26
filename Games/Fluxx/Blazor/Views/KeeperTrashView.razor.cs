namespace Fluxx.Blazor.Views;
public partial class KeeperTrashView
{
    [CascadingParameter]
    public KeeperStealViewModel? DataContext { get; set; }
}