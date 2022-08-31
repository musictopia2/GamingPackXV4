namespace MonopolyCardGame.Blazor;
public partial class ShowCardBlazor
{
    [CascadingParameter]
    public MonopolyCardGameVMData? GameData { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private string RealHeight => $"{TargetHeight}vh";
}