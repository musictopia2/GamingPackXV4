namespace Fluxx.Blazor;
public partial class RuleUI
{
    [CascadingParameter]
    public CompleteContainerClass? CompleteContainer { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private string RealHeight => $"{TargetHeight}vh";
}