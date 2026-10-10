namespace BasicGameFrameworkLibrary.Blazor.Views;
public class StateContentView : ComponentBase, IDisposable
{
    private IStateChanged? _subscribedDataContext;
    private bool _disposed;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    [EditorRequired]
    public IStateChanged DataContext { get; set; } = null!;

    protected override void OnParametersSet()
    {
        if (ReferenceEquals(_subscribedDataContext, DataContext) == false)
        {
            Unsubscribe();

            _subscribedDataContext = DataContext;
            _subscribedDataContext.StateChanged += Refresh;
        }

        base.OnParametersSet();
    }

    private void Refresh()
    {
        InvokeAsync(StateHasChanged);
    }

    private void Unsubscribe()
    {
        _subscribedDataContext?.StateChanged -= Refresh;
        _subscribedDataContext = null;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, ChildContent);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Unsubscribe();
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}