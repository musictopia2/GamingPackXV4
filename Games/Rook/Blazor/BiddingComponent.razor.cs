namespace Rook.Blazor;
public partial class BiddingComponent
{
    [Parameter]
    [EditorRequired]
    public RookMainViewModel? DataContext { get; set; }
}