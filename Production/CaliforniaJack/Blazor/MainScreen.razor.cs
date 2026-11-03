namespace CaliforniaJack.Blazor;
public partial class MainScreen : IHandle<OpenMainEventModel>,
    IHandle<CloseMainEventModel>, IDisposable
{
    private readonly IEventAggregator _aggregator;
    private CaliforniaJackMainViewModel? _dataContext;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public MainScreen()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
    }
    protected override void OnInitialized()
    {
        _aggregator.Publish(new AskMainEventModel());
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IDisposable.Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
    void IHandle<OpenMainEventModel>.Handle(OpenMainEventModel message)
    {
        _dataContext = (CaliforniaJackMainViewModel)message.DataContext;
        InvokeAsync(StateHasChanged);
    }

    void IHandle<CloseMainEventModel>.Handle(CloseMainEventModel message)
    {
        _dataContext = null;
        InvokeAsync(StateHasChanged);
    }
}