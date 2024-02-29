namespace MonopolyDicedGame.Blazor;
public partial class ValueSpaceComponent
{
    [Parameter]
    [EditorRequired]
    public int Value { get; set; }
    [Parameter]
    [EditorRequired]
    public string TargetHeight { get; set; } = "";
    private static string Color => cc1.White;
    private static string Border => cc1.Black;
}