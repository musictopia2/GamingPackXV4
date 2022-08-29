namespace Payday.Blazor.Views;
public partial class MailPileView
{
    [CascadingParameter]
    public PaydayVMData? VMData { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private string RealHeight => $"{TargetHeight}vh";
}