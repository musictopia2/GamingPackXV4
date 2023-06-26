namespace Fluxx.Blazor.Views;
public partial class KeeperStealView
{
    [CascadingParameter]
    public KeeperStealViewModel? DataContext { get; set; }
}