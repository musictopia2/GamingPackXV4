namespace Payday.Blazor.Views;
public partial class MailListView
{
    [CascadingParameter]
    public PaydayVMData? VMData { get; set; }
}