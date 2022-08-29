namespace Payday.Blazor.Views;
public partial class BasicPickerView<V>
    where V : BasicSubmitViewModel
{
    [CascadingParameter]
    public V? DataContext { get; set; }
    [CascadingParameter]
    public PaydayVMData? VMData { get; set; }
    private ICustomCommand SubmitCommand => DataContext!.SubmitCommand!;
}