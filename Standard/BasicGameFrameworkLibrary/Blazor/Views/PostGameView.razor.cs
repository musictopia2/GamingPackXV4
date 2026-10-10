namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class PostGameView<VM> : IHandle<OpenPostGameEventModel>,
    IHandle<ClosePostGameEventModel>, IDisposable
    where VM : class, IPostGameVM
{
    private VM? _dataContext;
    private readonly IEventAggregator _aggregator;
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    public PostGameView()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IDisposable.Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
    protected override void OnInitialized()
    {
        _aggregator.Publish(new AskPostGameEventModel());
    }
    void IHandle<OpenPostGameEventModel>.Handle(OpenPostGameEventModel message)
    {
        _dataContext = (VM)message.DataContext;
        InvokeAsync(StateHasChanged);
    }
    void IHandle<ClosePostGameEventModel>.Handle(ClosePostGameEventModel message)
    {
        _dataContext = null;
        InvokeAsync(StateHasChanged);
    }
}