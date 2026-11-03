namespace SorryDicedGame.Blazor;
public partial class SorrySpaceComponent
{
    [Parameter]
    [EditorRequired]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    [EditorRequired]
    public EnumColorChoice ColorUsed { get; set; }
    [Parameter]
    [EditorRequired]
    public bool HasPiece { get; set; }
    [Parameter]
    public EventCallback<EnumColorChoice> OnClick { get; set; }
    private void PrivateColorClick()
    {
        OnClick.InvokeAsync(ColorUsed);
    }
    private static SizeF TargetSize => new(40, 40);
}