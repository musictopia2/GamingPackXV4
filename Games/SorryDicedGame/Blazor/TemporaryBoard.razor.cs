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
    private static SizeF TargetSize => new(40, 40);
    [Parameter]
    public EventCallback<EnumColorChoice> OnChoseColor { get; set; }

    private void PrivateColorClick()
    {
        if (HowMany == 0)
        {
            return; //you cannot choose color because there was none found.
        }
        OnChoseColor.InvokeAsync(ColorUsed);
    }


}