namespace SorryDicedGame.Blazor;
public partial class PawnComponent
{
    [Parameter]
    [EditorRequired]
    public EnumColorChoice ColorUsed { get; set; }
    [Parameter]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    public int HowMany { get; set; }
    private static string Columns => gg1.RepeatAuto(2);
    private static SizeF TargetSize => new(40, 40);
}