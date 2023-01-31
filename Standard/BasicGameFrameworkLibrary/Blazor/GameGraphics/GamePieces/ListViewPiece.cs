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
            return true;
        }
        return _previousRecord!.Equals(GetRecord) == false;
    }
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }
    [Parameter]
    public ListPieceModel? DataContext { get; set; }
    [Parameter]
    public bool CanHighlight { get; set; } = true;
    [Parameter]
    public string TextColor { get; set; } = cs1.Navy;
    protected virtual void SelectProcesses() { }
    protected virtual bool CanDrawText()
    {
        return true;
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (CanDrawText() == false)
        {
            return;
        }
        SelectProcesses();
        ISvg svg = MainGraphics!.GetMainSvg();
        SvgRenderClass render = new();
        render.Allow0 = true;
        Text text = new();
        text.CenterText();
        text.Fill = TextColor.ToWebColor();
        text.Content = DataContext!.DisplayText;
        svg.Children.Add(text);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}
