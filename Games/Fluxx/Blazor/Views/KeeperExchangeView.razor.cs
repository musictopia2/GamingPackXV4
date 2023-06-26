namespace Fluxx.Blazor.Views;
public partial class KeeperExchangeView
{
    [CascadingParameter]
    public KeeperExchangeViewModel? DataContext { get; set; }
}