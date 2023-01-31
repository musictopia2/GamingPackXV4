namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.GamePieces;
internal record NumberRecord(string Display, string TextColor, bool Enabled, bool IsSelected);
public class NumberPiece : ComponentBase
{
    private NumberRecord? _previousRecord;
    protected override void OnAfterRender(bool firstRender)
    {
        _previousRecord = GetRecord;
        base.OnAfterRender(firstRender);
    }
    private NumberRecord GetRecord => new(GetValueToPrint(), TextColor, MainGraphics!.CustomCanDo.Invoke(), MainGraphics.IsSelected);
    protected override bool ShouldRender()
    {
        var current = GetRecord;
        return current.Equals(_previousRecord) == false;
    }
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; } 
    [Parameter]
    public NumberModel? DataContext { get; set; }
    protected virtual bool CanDrawNumber()
    {
        return true;
    }
    protected virtual void SelectProcesses() { }
    [Parameter]
    public bool CanHighlight { get; set; } = true;
    [Parameter]
    public string TextColor { get; set; } = cs1.Navy;
    protected virtual string GetValueToPrint() // so the overrided version can display something else.
    {
        if (DataContext == null)
        {
            return "";
        }
        if (DataContext!.NumberValue < 0)
        {
            return "";
        }
        return DataContext!.NumberValue.ToString();
    }
    protected virtual void OriginalSizeProcesses() { }
    protected override void OnInitialized()
    {
        MainGraphics!.NeedsHighlighting = CanHighlight;
        OriginalSizeProcesses();
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (CanDrawNumber() == false)
        {
            return;
        }
        string value = GetValueToPrint();
        if (value == "")
        {
            return;
        }
        SelectProcesses();
        ISvg svg = MainGraphics!.GetMainSvg();
        SvgRenderClass render = new();
        render.Allow0 = true;
        Text text = new();
        text.CenterText();
        if (value.Length == 3)
        {
            text.Font_Size = 20;
            text.PopulateStrokesToStyles();
        }
        else if (value.Length == 2)
        {
            text.Font_Size = 30;
            text.PopulateStrokesToStyles(strokeWidth: 2);
        }
        else
        {
            text.Font_Size = 40;
            text.PopulateStrokesToStyles(strokeWidth: 2);
        }
        text.Fill = TextColor.ToWebColor();
        text.Content = value;
        svg.Children.Add(text);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}