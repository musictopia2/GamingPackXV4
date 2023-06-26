namespace Fluxx.Blazor.Views;
public partial class KeeperShowView
{
    [CascadingParameter]
    public KeeperShowViewModel? DataContext { get; set; }
}