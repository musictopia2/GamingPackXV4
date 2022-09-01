namespace Fluxx.Blazor.Views;
public partial class ActionTradeHandsView : SimpleActionView
{
    [CascadingParameter]
    public ActionTradeHandsViewModel? DataContext { get; set; }
}