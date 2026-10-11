namespace Millebournes.Blazor.Views;
public partial class CoupeView //could have inherited and overrided but not worth redoing this one to do so.
{
    private ICustomCommand CoupeCommand => DataContext!.CoupeCommand!;
}