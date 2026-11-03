namespace Spades2Player.Blazor.Views;
public partial class SpadesBiddingView
{
    private ICustomCommand BidCommand => DataContext!.BidCommand!;
}