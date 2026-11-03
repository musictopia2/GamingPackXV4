namespace Spades4Player.Blazor;
public partial class BiddingComponent
{
    [Parameter]
    [EditorRequired]
    public Spades4PlayerMainViewModel? DataContext { get; set; }
    private ICustomCommand BidCommand => DataContext!.BidCommand!;
}