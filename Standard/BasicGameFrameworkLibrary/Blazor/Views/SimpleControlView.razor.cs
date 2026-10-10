namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class SimpleControlView<T> : IDisposable
    where T: SimpleControlObservable, IStateChanged
{
    private T? _subscribedDataContext;
    private bool _disposed;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    [EditorRequired]
    public T DataContext { get; set; } = null!;
    protected override void OnParametersSet()
    {
        if (ReferenceEquals(_subscribedDataContext, DataContext) == false)
        {
            Unsubscribe();

            _subscribedDataContext = DataContext;
            _subscribedDataContext.EnabledChanged += Refresh;
        }

        base.OnParametersSet();
    }

    private void Refresh()
    {
        InvokeAsync(StateHasChanged);
    }

    private void Unsubscribe()
    {
        _subscribedDataContext?.EnabledChanged -= Refresh;
        _subscribedDataContext = null;
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