namespace Millebournes.Blazor.Views;
public partial class CoupeView //could have inherited and overrided but not worth redoing this one to do so.
{
    [CascadingParameter]
    public CoupeViewModel? DataContext { get; set; }
    private ICustomCommand CoupeCommand => DataContext!.CoupeCommand!;
}