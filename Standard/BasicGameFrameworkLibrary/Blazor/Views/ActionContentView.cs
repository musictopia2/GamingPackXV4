namespace BasicGameFrameworkLibrary.Blazor.Views;
public class ActionContentView : ComponentBase, IDisposable
{
    private bool _disposed;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    [EditorRequired]
    public string Key { get; set; } = null!;
    private CommandContainer _container = null!;


    protected override void OnInitialized()
    {
        _container = Resolver!.Resolve<CommandContainer>();
        _container.AddSpecificAction(Key, Refresh);
        base.OnInitialized();
    }
    private void Refresh()
    {
        InvokeAsync(StateHasChanged);
    }
    private void Unsubscribe()
    {
        _container.RemoveSpecificAction(Key);
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