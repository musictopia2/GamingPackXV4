namespace Payday.Blazor.Views;
public partial class MailListView
{
    [CascadingParameter]
    public PaydayVMData? VMData { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; }
    private static string ContainerSize => $"{45.HeightPixels}px";
    private int HeightPixels => TargetHeight.HeightPixels;

}