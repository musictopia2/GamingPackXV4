namespace BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
internal record CenteredTextRecord(string Text, string BorderColor, bool Bold);
public class CenteredTextBlazor : ComponentBase
{
    private CenteredTextRecord? _previous;
    protected override void OnAfterRender(bool firstRender)
    {
        _previous = GetRecord;
        base.OnAfterRender(firstRender);
    }
    private CenteredTextRecord GetRecord => new(Text, BorderColor, Bold);
    [Parameter]
    public string Text { get; set; } = "";
    [Parameter]
    public float BorderWidth { get; set; }
    [Parameter]
    public string BorderColor { get; set; } = cs1.Transparent;
    [Parameter]
    public string TextColor { get; set; } = cs1.Black;
    [Parameter]
    public double FontSize { get; set; }
    [Parameter]
    public bool Bold { get; set; }
    protected override bool ShouldRender()
    {
        if (_previous is null)
        {
            return true;
        }
        return _previous!.Equals(GetRecord) == false;
    }
    [Parameter]
    public string FontFamily { get; set; } = "Lato";
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        SvgRenderClass render = new();
        ISvg svg = new SVG();
        render.Allow0 = true;
        Text text = new();
        text.CenterText();
        if (BorderWidth > 0 && BorderColor != cs1.Transparent)
        {
            text.PopulateStrokesToStyles(BorderColor.ToWebColor, BorderWidth, FontFamily);
        }
        else
        {
            text.Style = $"font-family: {FontFamily};";
        }
        text.Fill = TextColor.ToWebColor;
        text.Font_Size = FontSize;
        if (Bold)
        {
            text.Font_Weight = "bold";
        }
        text.Content = Text;
        svg.Children.Add(text);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}