namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.GamePieces;
internal record ListRecord(string Display, string Color, bool IsSelected, bool Enabled);
public class ListViewPiece : ComponentBase
{
    private ListRecord? _previousRecord;
    protected override void OnAfterRender(bool firstRender)
    {
        _previousRecord = GetRecord;
        base.OnAfterRender(firstRender);
    }
    private ListRecord GetRecord => new(DataContext!.DisplayText, TextColor, MainGraphics!.IsSelected, MainGraphics.CustomCanDo.Invoke());
    protected override bool ShouldRender()
    {
        if (_previousRecord == null)
        {
            return true; //try this way just in case.
        }
        return _previousRecord!.Equals(GetRecord) == false;
    }
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; } //still needed to get the start of the svg.  plus needs to have its start rectangle anyways.
    [Parameter]
    public ListPieceModel? DataContext { get; set; }
    [Parameter]
    public bool CanHighlight { get; set; } = true;
    [Parameter]
    public string TextColor { get; set; } = cs.Navy;
    protected virtual void SelectProcesses() { }
    protected virtual bool CanDrawText()
    {
        return true;
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (CanDrawText() == false)
        {
            return; //can't even continue because you can't draw.
        }
        SelectProcesses();
        ISvg svg = MainGraphics!.GetMainSvg();
        SvgRenderClass render = new();
        render.Allow0 = true; //try this.  because games like payday may need 0 to be allowed (?)
        Text text = new();
        text.CenterText();
        text.Fill = TextColor.ToWebColor();
        text.Content = DataContext!.DisplayText;
        svg.Children.Add(text);
        render.RenderSvgTree(svg, 0, builder);
        base.BuildRenderTree(builder);
    }
}
