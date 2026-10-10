namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.GamePieces;

//internal record ListRecord(
//    string Display,
//    string Color,
//    bool IsSelected,
//    bool Enabled);

public class ListViewPiece : ComponentBase, IDisposable
{
    //private ListRecord? _previousRecord;
    private ListPieceModel? _subscribedModel;
    private bool _disposedValue;

    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }

    [Parameter]
    public ListPieceModel? DataContext { get; set; }

    [Parameter]
    public bool CanHighlight { get; set; } = true;

    [Parameter]
    public string TextColor { get; set; } = cs1.Navy;

    //private ListRecord GetRecord => new(
    //    DataContext!.DisplayText,
    //    TextColor,
    //    DataContext.IsSelected,
    //    DataContext.IsEnabled);

    protected override void OnParametersSet()
    {
        if (ReferenceEquals(_subscribedModel, DataContext) == false)
        {
            UnsubscribeModel();

            if (DataContext is not null)
            {
                _subscribedModel = DataContext;
                _subscribedModel.StateChanged += Model_StateChanged;
            }
        }

        base.OnParametersSet();
    }

    private void Model_StateChanged()
    {
        InvokeAsync(StateHasChanged);
    }


    //protected override bool ShouldRender()
    //{
    //    if (_previousRecord is null)
    //    {
    //        return true;
    //    }

    //    return _previousRecord.Equals(GetRecord) == false;
    //}

    protected virtual void SelectProcesses()
    {
    }

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
        text.Fill = TextColor.ToWebColor;
        text.Content = DataContext!.DisplayText;

        svg.Children.Add(text);
        render.RenderSvgTree(svg, builder);

        base.BuildRenderTree(builder);
    }

    private void UnsubscribeModel()
    {
        _subscribedModel?.StateChanged -= Model_StateChanged;
        _subscribedModel = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            UnsubscribeModel();
        }

        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}