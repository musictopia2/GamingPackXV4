namespace Rook.Blazor;
public partial class BiddingComponent
{
    [Parameter]
    [EditorRequired]
    public RookMainViewModel? DataContext { get; set; }
    private ICustomCommand BidCommand => DataContext!.BidCommand!;
    private ICustomCommand PassCommand => DataContext!.PassCommand!;
}