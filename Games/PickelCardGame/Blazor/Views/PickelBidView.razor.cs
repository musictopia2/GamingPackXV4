namespace PickelCardGame.Blazor.Views;
public partial class PickelBidView
{
    [CascadingParameter]
    public PickelBidViewModel? DataContext { get; set; }
}