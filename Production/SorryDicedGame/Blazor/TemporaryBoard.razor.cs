namespace SorryDicedGame.Blazor;
public partial class TemporaryBoard
{
    [Parameter]
    [EditorRequired]
    public EnumColorChoice ColorUsed { get; set; }
    [Parameter]
    [EditorRequired]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    [EditorRequired]
    public int HowMany { get; set; }
    private static string Columns => gg1.RepeatAuto(2);
    [Parameter]
    public EventCallback<EnumColorChoice> OnChoseColor { get; set; }
    private void PrivateColorClick()
    {
        OnChoseColor.InvokeAsync(ColorUsed);
    }
}